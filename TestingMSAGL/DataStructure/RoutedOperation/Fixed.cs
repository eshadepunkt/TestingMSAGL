using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;

namespace TestingMSAGL.DataStructure.RoutedOperation
{
    public class Fixed : NodeComplex
    {
        public Fixed(GraphExtension graph, string name) : base(graph, name)
        {
            Subgraph.LabelText = "Fixed: " + Subgraph.Id;
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = Color.Gray;
        }
    }
}