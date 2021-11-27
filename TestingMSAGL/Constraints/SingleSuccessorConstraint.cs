using System.Reflection;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.Constraints
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