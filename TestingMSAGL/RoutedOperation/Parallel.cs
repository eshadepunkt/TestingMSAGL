using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Routing;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using Shape = Microsoft.Msagl.Drawing.Shape;

namespace TestingMSAGL.RoutedOperation
{
    public class Parallel : NodeComplex
    {
        public Parallel(GraphExtension graph, string name) : base(graph, name)
        {
            Subgraph.Attr.Shape = Shape.Parallelogram;
            Subgraph.Attr.FillColor = Color.YellowGreen;

        }
    }
}