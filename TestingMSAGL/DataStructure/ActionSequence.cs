using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace TestingMSAGL.DataStructure
{
    public class ActionSequence : IEnlistmentNotification
    {
        private bool _enlisted;
        private readonly Queue<ICompositeAction> _sequence = new ();
        private readonly Stack<ICompositeAction> _executedActions = new();

        public string Error { get; private set; }

        public void Enqueue(ICompositeAction action)
        {
            if (_enlisted)
            {
                throw new Exception("Cannot enqueue action after sequence enlisted for execution.");
            }
            
            _sequence.Enqueue(action);
        }
        

        public void Enlist()
        {
            var transaction = Transaction.Current;
            if (transaction == null)
                throw new Exception("Cannot execute ActionSequence without transaction!");
            _enlisted = true;
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
        }
        
        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Execute()
        {
            while(_sequence.Any())
            {
                var action = _sequence.Dequeue();
                if (!action.Perform())
                {
                    Error = action.Error;
                    Console.WriteLine("Forcing Rollback...");
                    throw new TransactionException();
                }
                _executedActions.Push(action);
            }

        }

        public void Commit(Enlistment enlistment)
        {
           enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            while (_executedActions.Any())
            {
                var action = _executedActions.Pop();
                action.Rollback();
            }
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }
    }
}