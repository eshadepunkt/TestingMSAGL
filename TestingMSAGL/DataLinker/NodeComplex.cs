using System;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.DataLinker
{
    public class NodeComplex : IWithId

    {
        private GraphExtension _graph;

        public NodeComplex(GraphExtension graph, string name)
        {
            var composite = new CompositeComplex { Name = name, DrawingNodeId = AddNode(graph) };
            Composite = composite;
            graph.AddNodeWithId(this);
            _graph = graph;
        }

        /// <summary>
        ///     Complex Node Drawing Representation
        /// </summary>
        internal Subgraph Subgraph { get; private set; }

        /// <summary>
        ///     Complex Node Representation
        /// </summary>
        internal CompositeComplex Composite { get; }

        // todo yet to decide if needed
        protected ComplexType ComplexType { get; set; }

        /// <summary>
        ///     Pass node id to Extern
        /// </summary>
        public string NodeId => Composite.DrawingNodeId;

        public string ParentId
        {
            get => Composite.ParentId;
            set => Composite.ParentId = value;
        }

        private string AddNode(Graph graph)
        {
            var nodeId = Guid.NewGuid().ToString();
            // todo implement proper fix for shortening the guuid
            Subgraph = new Subgraph(nodeId)
            {
                Attr = { FillColor = Color.Transparent, LabelMargin = 10, Padding = 10 },
                DiameterOfOpenCollapseButton = 10
            };
            Subgraph.LabelText = "Root " + Subgraph.Id.Split('-')[1];
            Subgraph.Label.FontSize = 8;
            Subgraph.Label.FontName = "New Courier";
            Subgraph.Label.FontColor = Color.Black;


            graph.AddNode(Subgraph);
            return nodeId;
        }

        /// <summary>
        ///     add a new Elementary to Complex node and return true on success
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
        ///     add a new Node to Complex node and return true on success
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool AddMember(IWithId node)
        {
            if (node is NodeComplex complex)
                if (Composite.AddMember(complex.Composite))
                {
                    Subgraph.AddSubgraph(complex.Subgraph);

                    complex.Composite.ParentId = Composite.DrawingNodeId;

                    return true;
                }

            if (node is NodeElementary elementary)
                if (Composite.AddMember(elementary.Composite))
                {
                    Subgraph.AddNode(elementary.Node);
                    elementary.Composite.ParentId = Composite.DrawingNodeId;
                    elementary.ParentId = Composite.DrawingNodeId;

                    return true;
                }

            return false;
        }


        public bool RemoveMember(IWithId child)
        {
            if (child is NodeElementary elementary) return Composite.RemoveMember(elementary.Composite);

            return false;
        }
    }
}