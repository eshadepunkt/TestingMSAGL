using System;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.DataLinker
{
    public class NodeComplex : IWithId

    {
        public NodeComplex(GraphExtension graph,  string name )
        {
            var composite = new CompositeComplex() { Name = name };
            composite.DrawingNodeId = AddNode(graph);
            Composite = composite;
            graph.AddNodeWithId(this);
        }

        private string AddNode(Graph graph)
        {
            var nodeId =  Guid.NewGuid().ToString();
            Subgraph = new Subgraph(nodeId);
            graph.AddNode(Subgraph);
            return nodeId;
        }
        /// <summary>
        /// Pass node id to Extern
        /// </summary>
        public string NodeId => Composite.DrawingNodeId;
        
        /// <summary>
        /// Complex Node Drawing Representation         
        /// </summary>
        internal Subgraph Subgraph { get; private set; }

        /// <summary>
        /// Complex Node Representation         
        /// </summary>
        internal CompositeComplex Composite { get; }

        /// <summary>
        /// add a new Elementary to Complex node and return true on success
        /// </summary>
        /// <param name="elementary"></param>
        /// <returns></returns>
        public bool AddMember(NodeElementary elementary)
        {
            if (Composite.AddMember(elementary.Composite))
            {
                Subgraph.AddNode(elementary.Node);
                return true;
            }
            return false;

            // TODO Maybe another member list is required here. . . 
            //TODO maybe some black magic for constraints
        
        }

        /// <summary>
        /// add a new Node to Complex node and return true on success
        /// </summary>
        /// <param name="elementary"></param>
        /// <returns></returns>
        public bool AddMember(NodeComplex elementary)
        {
            if (Composite.AddMember(elementary.Composite))
            {
                Subgraph.AddSubgraph(elementary.Subgraph);
                return true;
            }
            return false;
        }

        public Composite GetPredecessor()
        {
            return null;
        }

        //todo 
        public Composite GetSuccessor()
        {
            return null;
        }

        //todo am I the only one?

        //todo constraints


    }
}