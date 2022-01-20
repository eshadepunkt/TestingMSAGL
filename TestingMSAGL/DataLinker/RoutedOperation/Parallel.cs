using Microsoft.Msagl.Drawing;
using ComplexEditor.DataLinker;
using ComplexEditor.DataStructure;
using ComplexEditor.DataStructure.RoutingOperation;

namespace ComplexEditor.DataLinker.RoutedOperation
{
    public class Parallel : NodeComplex
    {
        public Parallel(GraphExtension graph, string name) : base(graph)
        {
            Composite = new ParallelRoutingOperation { Name = name, DrawingNodeId = AddNode(graph) };
            
            var color = Color.YellowGreen;
            color.A = base.tranparency;
            Subgraph.LabelText = "Parallel: " + Subgraph.Id.Split('-')[1];
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = color;
            graph.LayerConstraints.AddSameLayerNeighbors();
        }
    }
}