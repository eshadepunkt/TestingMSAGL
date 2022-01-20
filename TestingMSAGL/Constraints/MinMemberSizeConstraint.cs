using System.Reflection;
using ComplexEditor.DataStructure;

namespace ComplexEditor.Constraints
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