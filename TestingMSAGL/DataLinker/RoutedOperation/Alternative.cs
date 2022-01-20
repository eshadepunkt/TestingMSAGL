using System.ServiceModel.Channels;
using Microsoft.Msagl.Drawing;
using ComplexEditor.DataLinker;
using ComplexEditor.DataStructure;
using ComplexEditor.DataStructure.RoutingOperation;

namespace ComplexEditor.DataLinker.RoutedOperation
{
    public class Alternative : NodeComplex
    {
        public Alternative(GraphExtension graph, string name) : base(graph)
        {
            Composite = new AlternativeRoutingOperation { Name = name, DrawingNodeId = AddNode(graph) };
            
            var color = Color.Gold;
            color.A = base.tranparency;
            Subgraph.LabelText = "Alternative:\n " + Subgraph.Id.Split('-')[1];
            Subgraph.Attr.Shape = Shape.Box;
            Subgraph.Attr.FillColor = color;
        }
    }
}