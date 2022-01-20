using System.Linq;
using System.Reflection;

namespace ComplexEditor.Constraints
{
    public class OrCombinedConstraint : IConstraint
    {
        private readonly IConstraint[] _constraints;
        public OrCombinedConstraint(params IConstraint[] constraints)
        {
            this._constraints = constraints;
            this.Context = constraints[0].Context;
            // TODO: ensure all constraints have same context...
        }

        public MethodInfo Context { get; }
        
        public string ToOcl()
        {
            return string.Join(" or ", _constraints.Select(constraint => constraint.ToOcl()));
        }
        
    }
}