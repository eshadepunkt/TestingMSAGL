using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataStructure;
using TestingMSAGL.DataStructure.RoutingOperation;

namespace TestingMSAGL.DataLinker.RoutedOperation
{
    public class Fixed : NodeComplex
    {
        public Fixed(GraphExtension graph, string name) : base(graph)
        {
            Composite = new FixedRoutingOperation { Name = name, DrawingNodeId = AddNode(graph) };
            
            var color = Color.Gray;
            color.A = base.tranparency;
            Subgraph.LabelText = "Fixed: " + Subgraph.Id.Split('-')[1];
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = color;

            Composite.Type = "fixed";
        }
    }
}