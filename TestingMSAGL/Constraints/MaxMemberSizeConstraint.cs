using System.Reflection;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.Constraints
{
    public class MaxMemberSizeConstraint : IConstraint
    {
        private readonly int _max;
        
        public MaxMemberSizeConstraint(int max)
        {
            _max = max;
            Context = typeof(CompositeComplex).GetMethod("AddMember");
        }
        
        public MethodInfo Context { get; }
        
        public string ToOcl()
        {
            return $"self.Members.Count() < {_max}";
        }
    }
}