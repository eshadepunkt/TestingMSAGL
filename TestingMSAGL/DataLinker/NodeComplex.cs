using System;
using Microsoft.Msagl.Drawing;
using ComplexEditor.DataStructure;

namespace ComplexEditor.DataLinker
{
    public abstract class NodeComplex : IWithId

    {
        private readonly GraphExtension _graph;
        internal readonly byte tranparency = 98;

        protected NodeComplex(GraphExtension graph)
        {
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
        internal CompositeComplex Composite { get; init; }

        /// <summary>
        ///     Pass node id to Extern
        /// </summary>
        public string NodeId => Composite.DrawingNodeId;

        public string ParentId => Composite.Parent.DrawingNodeId;

        protected string AddNode(Graph graph)
        {
            var nodeId = Guid.NewGuid().ToString();
            // todo implement proper fix for shortening the guuid

            var color = Color.Gray;
            color.A = 75;
            

            Subgraph = new Subgraph(nodeId)
            {
                Attr = { FillColor = color, LabelMargin = 10, Padding = 10 },
                DiameterOfOpenCollapseButton = 5
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
        //public bool AddMember(NodeElementary elementary)
        //{
        //    if (Composite.AddMember(elementary.Composite))
        //    {
        //        elementary.ParentId = Composite.DrawingNodeId;
        //        Subgraph.AddNode(elementary.Node);
        //        return true;
        //    }

        //    return false;

        //    // TODO Maybe another member list is required here. . . 
        //    //TODO maybe some black magic for constraints
        //}

        /// <summary>
        ///     add a new Node to Complex node and return true on success
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool AddMember(IWithId node)
        {
            var result = node switch
            {
                NodeComplex complex => Composite.AddMember(complex.Composite),
                NodeElementary elementary => Composite.AddMember(elementary.Composite),
                _ => false
            };

            if(result) AddMemberToUi(node);

            return result;
        }

        public void AddMemberToUi(IWithId node)
        {
            switch (node)
            {
                case NodeComplex complex:
                    Subgraph.AddSubgraph(complex.Subgraph);
                    complex.Composite.Parent = Composite;
                    break;
                case NodeElementary elementary:
                    Subgraph.AddNode(elementary.Node);
                    elementary.Composite.Parent = Composite;
                    break;
            }
        }


        // TODO: Check that the element is actually a child.
        public bool RemoveMember(IWithId child)
        {
            var result = child switch
            {
                NodeElementary elementary => Composite.RemoveMember(elementary.Composite),
                NodeComplex complex => Composite.RemoveMember(complex.Composite),
                _ => false
            };

            if(result) RemoveMemberFromUi(child);

            return result;
        }

        public void RemoveMemberFromUi(IWithId child)
        {
            if (child is NodeElementary elementary)
                Subgraph.RemoveNode(elementary.Node);
        }
    }
}