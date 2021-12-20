using System;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.DataLinker

{
    public class NodeElementary : IWithId
    {
        public NodeElementary(GraphExtension graph, string name)
        {
            if (graph == null) return;
            var composite = new CompositeElementary { Name = name, DrawingNodeId = AddNode(graph, name) };
            Composite = composite;
            graph.AddNodeWithId(this);
        }

        public Node Node { get; private set; }
        public CompositeElementary Composite { get; }
        public string NodeId => Composite.DrawingNodeId;
        public string ParentId => Composite.Parent.DrawingNodeId;

        private string AddNode(Graph graph, string name = "")
        {
            var nodeId = Guid.NewGuid().ToString();
            Node = new Node(nodeId) { Attr = { LineWidth = 1, FillColor = Color.WhiteSmoke } };
            if (name != string.Empty)
                Node.LabelText = name;
            else
                Node.LabelText = "Label Nr. " + Node.Id.Split('-')[1];

            Node.Label.FontSize = 5;
            Node.Label.FontName = "New Courier";
            Node.Label.FontColor = Color.Blue;
            graph.AddNode(Node);
            return nodeId;
        }
    }
}