using System;
using System.Collections.Generic;

namespace TestingMSAGL.DataStructure
{
    public class CompositeComplex : Composite
    {
        public CompositeComplex()
        {
            Members = new List<Composite>();
        }

        public List<Composite> Members { get; }

        public bool AddMember(Composite composite)
        {
            Members.Add(composite);
            return true;
        }
    }
}