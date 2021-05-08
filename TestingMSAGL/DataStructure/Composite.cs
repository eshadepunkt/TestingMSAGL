namespace TestingMSAGL.DataStructure
{
    public abstract class Composite
    {
        public string Name { get; set; }
        public string DrawingNodeId { get; set; }
        public Composite Predecessor { get; set; }
        public Composite Successor { get; set; }
    }
}