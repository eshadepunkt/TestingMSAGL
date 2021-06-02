namespace TestingMSAGL.DataStructure
{
    public abstract class Composite : IComposite
    {
        public string Name { get; set; }
        public string DrawingNodeId { get; set; }
        public Composite Predecessor { get; set; }
        public Composite Successor { get; set; }

        public string ParentId { get; set; }
        public Composite ComplexComposite { get; }
    }
}