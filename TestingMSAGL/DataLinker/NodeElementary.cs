using System;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.DataLinker

{
    public class NodeElementary : IWithId
    {
        public NodeElementary(GraphExtension graph, string name)
        {
            var composite = new CompositeElementary { Name = name, DrawingNodeId = AddNode(graph) };
            Composite = composite;
            graph.AddNodeWithId(this);
        }

        public Node Node { get; private set; }
        public CompositeElementary Composite { get; }
        public string NodeId => Composite.DrawingNodeId;
        public string ParentId { get; set; }

        private string AddNode(Graph graph)
        {
            var nodeId = Guid.NewGuid().ToString();
            Node = new Node(nodeId) { Attr = { LineWidth = 1, FillColor = Color.WhiteSmoke } };
            Node.LabelText = "Label Nr. " + Node.Id.Split('-')[1];
            Node.Label.FontSize = 5;
            Node.Label.FontName = "New Courier";
            Node.Label.FontColor = Color.Blue;
            graph.AddNode(Node);
            return nodeId;
        }
    }
}