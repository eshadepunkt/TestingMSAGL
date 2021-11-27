using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using TestingMSAGL.DataStructure.RoutingOperation;

namespace TestingMSAGL.DataLinker.RoutedOperation
{
    public class Single : NodeComplex
    {
        public Single(GraphExtension graph, string name) : base(graph)
        {
            Composite = new SingleRoutingOperation { Name = name, DrawingNodeId = AddNode(graph) };

            var color = Color.DarkMagenta;
            color.A = base.tranparency;
            Subgraph.LabelText = "Single: " + Subgraph.Id.Split('-')[1];
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = color;

            Composite.Type = "single";
        }
    }
}