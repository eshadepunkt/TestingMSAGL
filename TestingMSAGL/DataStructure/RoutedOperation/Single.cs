using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;

namespace TestingMSAGL.DataStructure.RoutedOperation
{
    public class Single : NodeComplex
    {
        public Single(GraphExtension graph, string name) : base(graph, name)
        {
            Subgraph.LabelText = "Single: " + Subgraph.Id.Split('-')[1];
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = Color.DarkMagenta;
        }
    }
}