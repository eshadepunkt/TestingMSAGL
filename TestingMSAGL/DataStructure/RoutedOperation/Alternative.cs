using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;
using Shape = Microsoft.Msagl.Drawing.Shape;

namespace TestingMSAGL.DataStructure.RoutedOperation
{
    public class Alternative : NodeComplex
    {
        public Alternative(GraphExtension graph, string name) : base(graph, name)
        {
            Subgraph.LabelText = "Alternative: " + Subgraph.Id;
            Subgraph.Attr.Shape = Shape.InvHouse;
            Subgraph.Attr.FillColor = Color.Gold;
        }
        
    }
}