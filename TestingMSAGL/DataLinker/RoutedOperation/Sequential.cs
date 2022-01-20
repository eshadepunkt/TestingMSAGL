using Microsoft.Msagl.Drawing;
using ComplexEditor.DataLinker;
using ComplexEditor.DataStructure;
using ComplexEditor.DataStructure.RoutingOperation;

namespace ComplexEditor.DataLinker.RoutedOperation
{
    public class Sequential : NodeComplex
    {
        public Sequential(GraphExtension graph, string name) : base(graph)
        {
            Composite = new SequentialRoutingOperation() { Name = name, DrawingNodeId = AddNode(graph) };

            var color = Color.DarkMagenta;
            color.A = base.tranparency;
            Subgraph.LabelText = "Sequential: " + Subgraph.Id.Split('-')[1];
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = color;
        }
    }
}