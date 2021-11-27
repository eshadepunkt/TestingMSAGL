using System.Reflection;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.Constraints
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