using HarmonyLib;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.Patches
{
    [HarmonyPatch(typeof(Graph), "AddPrecalculatedEdge")]
    public class GraphPatch
    {
        static void Postfix(Edge edge)
        {
            var graph = MainWindow.Graph;

            if (edge.Source != edge.Target)
            {
                Composite sourceComposite = null;
                var sourceNode = graph.GetNodeById(edge.SourceNode.Id);
                if (sourceNode is NodeElementary elementary) sourceComposite = elementary.Composite;
                else if (sourceNode is NodeComplex complex) sourceComposite = complex.Composite;

                Composite targetComposite = null;
                var targetNode = graph.GetNodeById(edge.TargetNode.Id);
                if (targetNode is NodeElementary targetElementary) targetComposite = targetElementary.Composite;
                else if (targetNode is NodeComplex complex) targetComposite = complex.Composite;

                if (sourceComposite != null && targetComposite != null)
                {
                    sourceComposite.SetSuccessor(targetComposite);
                    targetComposite.SetPredecessor(sourceComposite);
                }
            }
        }
    }
}