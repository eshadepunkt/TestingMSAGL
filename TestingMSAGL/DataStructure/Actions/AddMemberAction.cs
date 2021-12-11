using System;

namespace TestingMSAGL.DataStructure.Actions
{
    public class AddMemberAction : ICompositeAction
    {
        public string Error { get; private set; }

        private readonly CompositeComplex _parent;
        private readonly Composite _child;

        public AddMemberAction(CompositeComplex parent, Composite child)
        {
            _parent = parent;
            _child = child;
        }
        
        public bool Perform()
        {
            var result = _parent.AddMember(_child);
            if (!result)
                Error = string.Join("\n", _parent.ConsumeErrors());
            return result;
        }

        public void Rollback()
        {
            _parent.RemoveMember(_child);
        }
    }
}