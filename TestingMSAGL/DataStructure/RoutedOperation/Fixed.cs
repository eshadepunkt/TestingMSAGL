using System.Drawing;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;
using Color = Microsoft.Msagl.Drawing.Color;
using Node = Microsoft.Msagl.Drawing.Node;
using Point = Microsoft.Msagl.Core.Geometry.Point;
using Rectangle = Microsoft.Msagl.Core.Geometry.Rectangle;
using Shape = Microsoft.Msagl.Drawing.Shape;

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