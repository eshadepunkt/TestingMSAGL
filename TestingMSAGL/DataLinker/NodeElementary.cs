using System;
using System.Linq;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.DataLinker

{
    public class NodeElementary
    {    
        public NodeElementary(Graph graph, CompositeElementary composite)
        {
            composite.DrawingNodeId = AddNode(graph);
            Composite = composite;
        }

        public Node Node { get; private set; }

        public CompositeElementary Composite { get; }

        private string AddNode(Graph graph)
        {
            var nodeId = new Guid().ToString();
            Node = new Node(nodeId);
            graph.AddNode(Node);
            Node.Attr.LineWidth = 1;
            Node.LabelText = "Label Nr. " + Node.Id.Split('-')[1];
            Node.Label.FontSize = 5;
            Node.Label.FontName = "New Courier";
            Node.Label.FontColor = Color.Blue;
            return nodeId;
        }
    }
}