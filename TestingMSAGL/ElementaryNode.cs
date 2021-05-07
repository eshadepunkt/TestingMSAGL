using System.Dynamic;
using System.Linq;
using Microsoft.Msagl.Drawing;

namespace TestingMSAGL

{
    public class ElementaryNode
    {
        private Node Node { get; }
        private IViewerNode Successor { get; set; }
        private IViewerNode Predecessor { get; set; }
        //private Elementary Elementary { get; }

        // public ElementaryNode(Node node, Elementary elementary)
        // {
        //     Node = node;
        //     Elementary = elementary;
        // }
        public ElementaryNode(Node node)
        {
            Node = node;
        }

        public IViewerObject getPredecessor()
        {
            return null;
        }
        public IViewerObject getSuccessor()
        {
            return null;
        }

        private bool isSingleEdgeINandOut()
        {
            return (Node.OutEdges.Count() < 2 && Node.InEdges.Count() < 2);

        }
    }
}