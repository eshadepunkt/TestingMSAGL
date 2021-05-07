using System;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;

namespace TestingMSAGL
{
    public class ComplexNode
    {
        public Subgraph Subgraph { get;  }        
        //private Complex Complex { get;  }  
        private IViewerNode Successor { get; set; }
        private IViewerNode Predecessor { get; set; }

        // public ComplexNode(Subgraph subgraph, Complex complex)
        // {
        //     Subgraph = subgraph;
        //     Complex = complex;
        // }
        public ComplexNode(Subgraph subgraph)
        {
            Subgraph = subgraph;
        }
        //todo 

        public IViewerNode getPredecessor()
        {
            return null;
        }
        //todo 
        public IViewerNode getSuccessor()
        {
            return null;
        }
        
        //todo am I the only one?
        
        //todo constraints


        
    }
}