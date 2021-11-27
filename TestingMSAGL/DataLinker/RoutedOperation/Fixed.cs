using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;

namespace TestingMSAGL.DataStructure.RoutedOperation
{
    public class Fixed : NodeComplex
    {
        public Fixed(GraphExtension graph, string name) : base(graph, name)
        {
            var color = Color.Gray;
            color.A = base.tranparency;
            Subgraph.LabelText = "Fixed: " + Subgraph.Id.Split('-')[1];
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = color;

            Composite.Type = "fixed";
        }
    }
}