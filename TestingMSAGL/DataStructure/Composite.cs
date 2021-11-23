namespace TestingMSAGL.DataStructure
{
    public abstract class Composite : IComposite
    {
        public string Name { get; set; }
        public string DrawingNodeId { get; set; }
        public Composite Predecessor { get; private set; }
        public Composite Successor { get; private set; }

        public string ParentId { get; set; }
        public Composite ComplexComposite { get; }
        public void AddPredecessor(Composite node)
        {
            Predecessor = node; 
        }
        public void AddSuccessor(Composite node)
        {
            Successor = node;
        }
       
    }
}