using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;

namespace TestingMSAGL.DataStructure
{
    public class GraphExtension : Graph
    {
        public readonly HashSet<IWithId> DataLinkerNodes = new();
        
        public NodeComplex RootNode { set; get; }

        public GraphExtension(string label, string id) : base(label, id)
        {
        }

        /// <summary>
        ///     May need catch if node is not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IWithId GetNodeById(string id)
        {
            return DataLinkerNodes.Single(x => x.NodeId.Equals(id));
        }

        /// <summary>
        ///     Reduce casting
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NodeComplex GetComplexNodeById(string id)
        {
            return DataLinkerNodes.Single(x => x.NodeId.Equals(id)) as NodeComplex;
        }


        public bool DeleteById(string id)
        {
            var toBeDeleted = DataLinkerNodes.SingleOrDefault(x => x.NodeId.Equals(id));
            return DataLinkerNodes.Remove(toBeDeleted);
        }

        /// <summary>
        ///     Returns false if item is already present.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddNodeWithId(IWithId item)
        {
            return DataLinkerNodes.Add(item);
        }

        public bool DeleteRecursive(Subgraph subgraph)
        {
            // check if there are children subgraphs to dive into
            var toBeDeleted = new List<Node>();
            if (subgraph is null) return false;

            var listOfChildren = subgraph.AllSubgraphsWidthFirstExcludingSelf().ToList();
            if (subgraph.Subgraphs.Any())
                foreach (var child in listOfChildren)
                    DeleteRecursive(subgraph);
            if (subgraph.Nodes.Any())
            {
                toBeDeleted.AddRange(subgraph.Nodes);
                foreach (var node in toBeDeleted)
                {
                    //todo naming!
                    var elementary = GetNodeById(node.Id);
                    var complex = GetComplexNodeById(elementary.ParentId);
                    complex.RemoveMember(elementary);

                    subgraph.RemoveNode(node);
                    DeleteById(node.Id);
                    RemoveNode(node);
                }
            }

            //todo naming!
            var parent = GetComplexNodeById(subgraph.Id);
            var grandparent = GetComplexNodeById(parent.NodeId);
            grandparent.RemoveMember(parent);

            subgraph.ParentSubgraph.RemoveSubgraph(subgraph);
            DeleteById(subgraph.Id);
            RemoveNode(subgraph);


            return true;


            // if check so, call this method again
        }

        public void EnforceConstraints(Subgraph subgraph)
        {
            if (subgraph.Nodes.Count() > 1 && subgraph.Subgraphs.Count() > 1)
            {
                var allNodes = subgraph.Nodes.ToArray();
                var allSubgraphs = subgraph.Subgraphs.ToArray();

                LayerConstraints.AddSameLayerNeighbors(allNodes);
                LayerConstraints.AddSameLayerNeighbors(allSubgraphs);
            }
        }

      

    }
}