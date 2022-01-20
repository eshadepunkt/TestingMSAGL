using System.Reflection;
using ComplexEditor.DataStructure;

namespace ComplexEditor.Constraints
{
    public class SinglePredecessorConstraint : IConstraint
    {
        public SinglePredecessorConstraint()
        {
            Context = typeof(Composite).GetMethod("SetPredecessor");
        }

        public MethodInfo Context { get; }
        
        public string ToOcl()
        {
            return "self.Predecessor = null or predecessor = null";
        }
    }
}