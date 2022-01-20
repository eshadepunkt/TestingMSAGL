using System.Reflection;

namespace ComplexEditor.Constraints
{
    public interface IConstraint
    {
        public string ToOcl();
        public MethodInfo Context { get; }
    }
}