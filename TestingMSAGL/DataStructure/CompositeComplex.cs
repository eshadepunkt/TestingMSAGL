using System;
using System.Collections.Generic;
using System.Linq;

namespace ComplexEditor.DataStructure
{
    public abstract class CompositeComplex : Composite
    {
        protected CompositeComplex()
        {
            Members = new HashSet<Composite>();
        }

        /// <summary>
        ///     Contains all inherent Composites
        /// </summary>
        public HashSet<Composite> Members { get; }

        /// <summary>
        ///     Adds a new Composite
        /// </summary>
        /// <param name="composite"></param>
        /// <returns>true on success</returns>
        public bool AddMember(Composite composite)
        {
            if (Errors.Count != 0)
                return false;
            return Members.Add(composite);
        }

        public bool RemoveMember(Composite composite)
        {
            if (Errors.Count != 0)
                return false;
            return Members.Remove(composite);
        }
        
        public Composite BreadthFirstSearch(Predicate<Composite> searchQuery)
        {
            var children = new Queue<Composite>(Members);
            while (children.Any())
            {
                var child = children.Dequeue();
                if (searchQuery.Invoke(child))
                    return child;

                if (child is not CompositeComplex complexChild)
                    continue;
                
                foreach (var childOfChild in complexChild.Members)
                    children.Enqueue(childOfChild);
            }

            return null;
        }
    }
}