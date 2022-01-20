using System.Reflection;
using ComplexEditor.DataStructure;

namespace ComplexEditor.Constraints
{
    public class SingleSuccessorConstraint : IConstraint
    {
        public SingleSuccessorConstraint()
        {
            Context = typeof(Composite).GetMethod("SetSuccessor");
        }

        public MethodInfo Context { get; }
        
        public string ToOcl()
        {
            return "self.Successor = null or successor = null";
        }
    }
}