using System;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.DataLinker
{
    public class NodeComplex : IWithId

    {
        public NodeComplex(GraphExtension graph, CompositeComplex composite)
        {
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
            Subgraph.AddNode(elementary.Node);
            Composite.AddMember(elementary.Composite);

            // TODO Maybe another member list is required here. . . 

            //TODO maybe some black magic for constraints
            
            return true;
        }

        /// <summary>
        /// add a new Node to Complex node and return true on success
        /// </summary>
        /// <param name="elementary"></param>
        /// <returns></returns>
        public bool AddMember(NodeComplex elementary)
        {
            Subgraph.AddSubgraph(elementary.Subgraph);
            Composite.AddMember(elementary.Composite);
            //TODO maybe some black magic for constraints
            
            return true;
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