using System.Reflection;
using OCL.Absyn;

namespace TestingMSAGL.Constraints
{
    public class NotConstraint : IConstraint
    {
        private readonly IConstraint _constraint;

        public NotConstraint(IConstraint constraint)
        {
            _constraint = constraint;
            Context = _constraint.Context;
        }
        
        public string ToOcl()
        {
            return "not " + _constraint.ToOcl();
        }

        public MethodInfo Context { get; }
    }
}