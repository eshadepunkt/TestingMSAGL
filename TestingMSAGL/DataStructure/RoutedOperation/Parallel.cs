using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;
using Shape = Microsoft.Msagl.Drawing.Shape;

namespace TestingMSAGL.DataStructure.RoutedOperation
{
    public class Parallel : NodeComplex
    {
        public Parallel(GraphExtension graph, string name) : base(graph, name)
        {
            Subgraph.LabelText = "Parallel: " + Subgraph.Id;
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = Color.YellowGreen;            
            graph.LayerConstraints.AddSameLayerNeighbors();

        }
    }
}