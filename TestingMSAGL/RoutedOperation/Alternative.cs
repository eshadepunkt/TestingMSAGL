using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using Shape = Microsoft.Msagl.Drawing.Shape;

namespace TestingMSAGL.RoutedOperation
{
    public class Alternative : NodeComplex
    {
        public Alternative(GraphExtension graph, string name) : base(graph, name)
        {
            Subgraph.Attr.Shape = Shape.Diamond;
            Subgraph.Attr.FillColor = Color.Gold;
        }
    }
}