namespace ComplexEditor.DataLinker
{
    public interface IWithId
    {
        string NodeId { get; }
        string ParentId { get; }
    }
}