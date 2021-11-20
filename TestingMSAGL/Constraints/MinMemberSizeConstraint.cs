using System.Reflection;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.Constraints
{
    public class MinMemberSizeConstraint : IConstraint
    {
        private readonly int _min;

        public MinMemberSizeConstraint(int min)
        {
            _min = min;           
            Context = typeof(CompositeComplex).GetMethod("RemoveMember");
        }
        
        public MethodInfo Context { get; }

        public string ToOcl()
        {
            return $"self.Members.Count() > {_min}";
        }
    }
}