using System;
using System.Reflection;
using ComplexEditor.DataStructure;

namespace ComplexEditor.Constraints
{
    public class TypeConstraint : IConstraint
    {
        private readonly Type _type;

        public TypeConstraint(Type type)
        {
            _type = type;
            Context = typeof(CompositeComplex).GetMethod("RemoveMember");
        }

        public MethodInfo Context { get; }
        
        public string ToOcl()
        {
            return "\"(self is " + _type.Name + ")\"";
        }
    }
}