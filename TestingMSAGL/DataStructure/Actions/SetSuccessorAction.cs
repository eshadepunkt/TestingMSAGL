using System;

namespace ComplexEditor.DataStructure.Actions
{
    public class SetSuccessorAction : ICompositeAction
    {
        public string Error { get; private set; }

        private readonly Composite _source, _target;
        private Composite _previousSuccessor;
        
        public SetSuccessorAction(Composite source, Composite target)
        {
            _source = source;
            _target = target;
        }
        
        public bool Perform()
        {
            _previousSuccessor = _source.Successor;
            var result = _source.SetSuccessor(_target);
            if (!result)
                Error = string.Join("\n", _source.ConsumeErrors());
            
            return result;
        }

        public void Rollback()
        {
            _source.SetSuccessor(_previousSuccessor);
        }
    }
}