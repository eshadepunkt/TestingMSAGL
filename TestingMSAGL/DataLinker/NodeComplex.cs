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
            Subgraph = new Subgraph(nodeId) {Attr = {FillColor = Color.Beige, LabelWidthToHeightRatio = 0.5}};
          
            graph.AddNode(Subgraph);
            return nodeId;
        }
        /// <summary>
        /// Pass node id to Extern
        /// </summary>
        public string NodeId => Composite.DrawingNodeId;

        public string ParentId
        {
            get => Composite.ParentId;
            set => Composite.ParentId = value;
        }

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
                elementary.ParentId = Composite.DrawingNodeId;
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
        /// <param name="node"></param>
        /// <returns></returns>
        public bool AddMember(IWithId node)
        {
            if (node is NodeComplex complex)
            {
                if (Composite.AddMember(complex.Composite))
                {
                   
                    Subgraph.AddSubgraph(complex.Subgraph);
                    
                    complex.Composite.ParentId = Composite.DrawingNodeId;

                    return true;
                }
            }

            if (node is NodeElementary elementary)
            {
                if (Composite.AddMember(elementary.Composite))
                {
                   
                    Subgraph.AddNode(elementary.Node);
                    elementary.Composite.ParentId = Composite.DrawingNodeId;
                    
                    return true;
                }
            }
            return false;
        }
        
    
        public bool RemoveMember(IWithId child)
        {
            if (child is NodeComplex complex)
            {
                return Composite.RemoveMember(complex.Composite);
            }

            return false;
        }
    }
}