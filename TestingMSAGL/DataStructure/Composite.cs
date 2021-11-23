namespace TestingMSAGL.DataStructure
{
    public abstract class Composite : IComposite
    {
        public string Name { get; set; }
        public string DrawingNodeId { get; set; }
        public Composite Predecessor { get; private set; }
        public Composite Successor { get; private set; }
        
        public string Type { get; set; }

        public string ParentId { get; set; }
        public Composite ComplexComposite { get; }

        public void SetSuccessor(Composite successor)
        {
            Successor = successor;
        }

        public void SetPredecessor(Composite predecessor)
        {
            Predecessor = predecessor;
        }
    }
}