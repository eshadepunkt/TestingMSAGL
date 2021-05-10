using System.Collections.Generic;

namespace TestingMSAGL.DataStructure
{
    public class CompositeComplex : Composite
    {
        public CompositeComplex()
        {
            Members = new HashSet<Composite>();
        }

        /// <summary>
        /// Contains all inherent Composites
        /// </summary>
        public HashSet<Composite> Members { get; }

        /// <summary>
        /// Adds a new Composite
        /// </summary>
        /// <param name="composite"></param>
        /// <returns>true on success</returns>
        public bool AddMember(Composite composite)
        {
            return Members.Add(composite);
        }
        
    }
}