using System.Reflection;
using TestingMSAGL.DataStructure;

namespace TestingMSAGL.Constraints
{
    public class TypeConstraint : IConstraint
    {
        private readonly string _type;
        private readonly Mode _mode;

        public enum Mode
        {
            Equals,
            NotEquals
        }
        
        public TypeConstraint(string type, Mode mode)
        {
            _type = type;
            _mode = mode;
            Context = typeof(CompositeComplex).GetMethod("AddMember");
        }

        public MethodInfo Context { get; }
        
        public string ToOcl()
        {
            if (_mode == Mode.Equals)
                return "composite.Type = '" + _type + "'";
            else
                return "composite.Type <> '" + _type + "'";
        }
    }
}