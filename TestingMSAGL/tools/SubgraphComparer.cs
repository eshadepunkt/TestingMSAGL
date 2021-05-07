using System;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;

namespace TestingMSAGL
{
    internal class SubgraphComparer :  Comparer<Subgraph>

    {
        public override int Compare(Subgraph x, Subgraph y)
        {
            var IdOfSubgraphA = Convert.ToInt32(x.Id.Split(':')[2]);
            var IdOfSubgraphB = Convert.ToInt32(y.Id.Split(':')[2]);

            return IdOfSubgraphA.CompareTo(IdOfSubgraphB);
        }
    }
}