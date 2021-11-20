using System.Reflection;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.Constraints
{
    public class DifferentTasksConstraint : IConstraint
    {
        public DifferentTasksConstraint()
        {
            Context = typeof(CompositeComplex).GetMethod("AddMember");
        }
        
        public MethodInfo Context { get; }

        // This should check composite IDs instead of their name (currently there are no IDs)
        public string ToOcl()
        {
            return "string.IsNullOrEmpty(composite.Name) or self.Members->forAll(c|c.Name <> composite.Name)";
        }
    }
}