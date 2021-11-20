using System.Reflection;

namespace TestingMSAGL.Constraints
{
    public interface IConstraint
    {
        public string ToOcl();
        public MethodInfo Context { get; }
    }
}