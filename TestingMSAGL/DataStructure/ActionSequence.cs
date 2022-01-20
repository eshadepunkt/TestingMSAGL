using System.Collections.Generic;
using System.Linq;

namespace ComplexEditor.DataStructure
{
    public class ActionSequence
    {
        private readonly Queue<ICompositeAction> _sequence = new ();
        private readonly Stack<ICompositeAction> _executedActions = new();
        private bool _success = true;

        public string Error { get; private set; }

        public void Enqueue(ICompositeAction action)
        {
            _sequence.Enqueue(action);
        }

        public bool Execute()
        {
            while(_sequence.Any())
            {
                var action = _sequence.Dequeue();
                if (!action.Perform())
                {
                    Error = action.Error;
                    _success = false;
                    Rollback();
                    break;
                }
                _executedActions.Push(action);
            }

            return _success;
        }

        public void Rollback()
        {
            while (_executedActions.Any())
            {
                var action = _executedActions.Pop();
                action.Rollback();
            }
        }
    }
}