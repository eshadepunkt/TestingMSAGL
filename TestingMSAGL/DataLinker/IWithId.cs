namespace TestingMSAGL.DataLinker
{
    public interface IWithId
    {
        string NodeId { get; }
        string ParentId { get; set; }
    }
}