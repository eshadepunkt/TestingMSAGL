using System.Collections.Generic;

namespace TestingMSAGL.DataStructure
{
    public abstract class Composite : IComposite
    {
        public string Name { get; set; }
        public string DrawingNodeId { get; set; }
        public Composite Predecessor { get; private set; }
        public Composite Successor { get; private set; }
        
        public string Type { get; set; }

        public Composite Parent { get; set; }
        public Composite ComplexComposite { get; }

        // This member must be public because errors are set by the OCL constraint provider
        public List<string> Errors { get; } = new();
        
        public IEnumerable<string> ConsumeErrors()
        {
            while (Errors.Count > 0)
            {
                var error = Errors[0];
                Errors.RemoveAt(0);
                yield return error;
            }
        }

        public bool SetSuccessor(Composite successor)
        {
            if (Errors.Count != 0)
                return false;
            Successor = successor;
            return true;
        }

        public bool SetPredecessor(Composite predecessor)
        {
            if (Errors.Count != 0)
                return false;
            Predecessor = predecessor;
            return true;
        }
    }
}