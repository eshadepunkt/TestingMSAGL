using System;
using System.Windows;
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
            var editor = MainWindow.Editor;

            if (edge.Source != edge.Target)
            {
                Composite sourceComposite = null;
                var sourceNode = editor.Graph.GetNodeById(edge.SourceNode.Id);
                if (sourceNode is NodeElementary elementary) sourceComposite = elementary.Composite;
                else if (sourceNode is NodeComplex complex) sourceComposite = complex.Composite;

                Composite targetComposite = null;
                var targetNode = editor.Graph.GetNodeById(edge.TargetNode.Id);
                if (targetNode is NodeElementary targetElementary) targetComposite = targetElementary.Composite;
                else if (targetNode is NodeComplex complex) targetComposite = complex.Composite;
                if (sourceComposite != null && targetComposite != null)
                {
                    if (!sourceComposite.SetSuccessor(targetComposite))
                    {
                        Console.WriteLine("Edge constraint failed");
                        editor.Graph.RemoveEdge(edge);
                        editor.refreshLayout();
                        MessageBox.Show("Could not add edge\n" + string.Join(", ", sourceComposite.ConsumeErrors()));
                        return;
                    }

                    if(!targetComposite.SetPredecessor(sourceComposite))
                    {
                        Console.WriteLine("Edge constraint failed");
                        editor.Graph.RemoveEdge(edge);
                        editor.refreshLayout();
                        MessageBox.Show("Could not add edge\n" + string.Join(", ", targetComposite.ConsumeErrors()));
                    }
                }
            }
        }
    }
}