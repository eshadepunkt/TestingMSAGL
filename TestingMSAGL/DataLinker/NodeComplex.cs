using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataStructure;
using Edge = Microsoft.Msagl.Drawing.Edge;
using Node = Microsoft.Msagl.Drawing.Node;

namespace TestingMSAGL.DataLinker
{
    public class NodeComplex : IWithId

    {
        public NodeComplex(GraphExtension graph,  string name )
        {
            var composite = new CompositeComplex {Name = name, DrawingNodeId = AddNode(graph)};
            Composite = composite;
            graph.AddNodeWithId(this);
        }

        private string AddNode(Graph graph)
        {
            var nodeId =  Guid.NewGuid().ToString();
            Subgraph = new Subgraph(nodeId) {Attr = {FillColor = Color.Beige, Padding = 20}};
            /*if (!Subgraph.Nodes.Any())
            {
                var node = new Node("Dummy") {IsVisible = true};
                Subgraph.AddNode(node);
            }*/
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

        // todo yet to decide if needed
        protected ComplexType ComplexType { get; set; } 

        /// <summary>
        /// add a new Elementary to Complex node and return true on success
        /// </summary>
        /// <param name="elementary"></param>
        /// <returns></returns>
        public bool AddMember(NodeElementary elementary)
        {
            
            if (Composite.AddMember(elementary.Composite))
            {
                elementary.Composite.ParentId = Composite.DrawingNodeId;
                Subgraph.AddNode(elementary.Node);
                return true;
            }
            return false;

            // TODO Maybe another member list is required here. . . 
            //TODO maybe some black magic for constraints
        
        }
        private void SetFormat(ComplexType format)
            {
                ComplexType = format;
            }



        /// <summary>
        /// add a new Node to Complex node and return true on success
        /// </summary>
        /// <param name="elementary"></param>
        /// <returns></returns>
        public bool AddMember(IWithId elementary)
        {
            var complex = elementary as NodeComplex;
          
            
            if (Composite.AddMember(complex.Composite))
            {
                Subgraph.AddSubgraph(complex.Subgraph);
                complex.Composite.ParentId = Composite.DrawingNodeId;

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

        public static NodeElementary GetParent(GraphExtension graph, NodeElementary elementary)
        {
            return null;

        }

        public NodeComplex GetChild(GraphExtension graph, NodeComplex complex)
        {
            return null;
        }
        //todo am I the only one?

        //todo constraints


        public bool RemoveMember(IWithId child)
        {
            List<Subgraph> subgraphs = new List<Subgraph>();
            List<Node> nodes = new List<Node>();
            
            var nodeComplex = child as NodeComplex;
        
                if (Composite.RemoveChildOfMember(nodeComplex?.Composite) )
                {
                    if(nodeComplex.Subgraph.Nodes.Any())
                        foreach (var subgraphNode in nodeComplex.Subgraph.Nodes)
                        {
                            nodes.Add(subgraphNode);
                        }
                    if(Subgraph.Subgraphs.Any())
                        foreach (var subgraph in nodeComplex.Subgraph.Subgraphs)
                        {
                            subgraphs.Add(subgraph);
                        }
                    nodes.RemoveAll(x=>x.Equals(x));

                    subgraphs.RemoveAll(x=>x.Equals(x));
                    Subgraph.RemoveSubgraph(nodeComplex?.Subgraph);
                    return true;
                }
            return false;
        }

        public bool RemoveChild(IWithId child)
        {
            var childNode = child as NodeComplex;
            //if (Composite.RemoveMember())
            //{
                   // Subgraph.RemoveNode(child?.);
                
                    //Subgraph.RemoveSubgraph(child.Node );
              
            //}
            return true;
        }


    }
}