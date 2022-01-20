using System;
using System.Windows;
using HarmonyLib;
using Microsoft.Msagl.Drawing;
using ComplexEditor.DataLinker;
using ComplexEditor.DataStructure;

namespace ComplexEditor.Patches
{
    [HarmonyPatch(typeof(Graph), "AddPrecalculatedEdge")]
    public class GraphPatch
    {
        static void Postfix(Edge edge)
        {
            var editor = UserControl1.Editor;
            editor.RegisterEdge(edge);
        }
    }
}