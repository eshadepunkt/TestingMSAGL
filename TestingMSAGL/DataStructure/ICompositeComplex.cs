namespace TestingMSAGL.DataStructure
{
    public interface ICompositeComplex
    {
        string DrawingNodeId { get; set; }
        Composite Predecessor { get; set; }
        Composite Successor { get; set; }
        string ParentId { get; set; }
    }
}