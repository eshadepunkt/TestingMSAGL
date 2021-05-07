using System;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;

namespace TestingMSAGL
{
    internal class NodeComparer : Comparer<Node>
    {
        public override int Compare(Node nodeA, Node nodeB)
        {
            var IdOfNodeA = Convert.ToInt32(nodeA.Id.Split(':')[1]);
            var IdOfNodeB = Convert.ToInt32(nodeB.Id.Split(':')[1]);

            return IdOfNodeA.CompareTo(IdOfNodeB);
        }
    }
}