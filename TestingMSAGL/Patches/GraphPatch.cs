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
            editor.RegisterEdge(edge);
        }
    }
}