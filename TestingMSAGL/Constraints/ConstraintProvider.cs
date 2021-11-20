using System.Collections.Generic;
using System.Linq;

namespace TestingMSAGL.Constraints
{
    public class ConstraintProvider
    {
        public static string GenerateOcl(IEnumerable<IConstraint> constraints)
        {
            var ocl = "";
            var groupedConstraints = constraints.GroupBy(constraint => constraint.Context);
            foreach (var group in groupedConstraints)
            {
                var context = group.Key;
                ocl += "context " + context.DeclaringType.Name + "::" + context.Name + "(";
                ocl += string.Join(", ", context.GetParameters().Select(parameter => parameter.Name + " : " + parameter.ParameterType.Name));
                ocl += ")\n";
                
                foreach (var constraint in group)
                {
                    ocl += "  pre " + constraint.GetType().Name + ":\n";
                    ocl += "    " + constraint.ToOcl() + "\n";
                }
            }

            return ocl;
        }
    }
}