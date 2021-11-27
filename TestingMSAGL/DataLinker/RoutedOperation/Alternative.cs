using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;

namespace TestingMSAGL.DataStructure.RoutedOperation
{
    public class Alternative : NodeComplex
    {
        public Alternative(GraphExtension graph, string name) : base(graph, name)
        {
            var color = Color.Gold;
            color.A = base.tranparency;
            Subgraph.LabelText = "Alternative:\n " + Subgraph.Id.Split('-')[1];
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = color;

            Composite.Type = "alternative";
        }
    }
}