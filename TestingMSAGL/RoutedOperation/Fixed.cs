using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Routing;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using Shape = Microsoft.Msagl.Drawing.Shape;

namespace TestingMSAGL.RoutedOperation
{
    public class Fixed : NodeComplex
    {
        public Fixed(GraphExtension graph, string name) : base(graph, name)
        {
            Subgraph.Attr.Shape = Shape.Ellipse;
            Subgraph.Attr.FillColor = Color.Gray;
            
        }
    }
}