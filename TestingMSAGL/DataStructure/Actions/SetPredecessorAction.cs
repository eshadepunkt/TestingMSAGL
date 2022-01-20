using System;

namespace ComplexEditor.DataStructure.Actions
{
    public class SetPredecessorAction : ICompositeAction
    {
        public string Error { get; private set; }

        private readonly Composite _source, _target;
        private Composite _previousPredecessor;

        public SetPredecessorAction(Composite source, Composite target)
        {
            _source = source;
            _target = target;
        }
        
        public bool Perform()
        {
            _previousPredecessor = _source.Predecessor;
            var result = _source.SetPredecessor(_target);
            if (!result)
                Error = string.Join("\n", _source.ConsumeErrors());
            
            return result;
        }

        public void Rollback()
        {
            _source.SetPredecessor(_previousPredecessor);
        }
    }
}