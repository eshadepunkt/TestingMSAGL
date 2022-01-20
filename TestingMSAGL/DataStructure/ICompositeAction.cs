namespace ComplexEditor.DataStructure
{
    public interface ICompositeAction
    {
        public string Error { get; }
        public bool Perform();
        public void Rollback();
    }
}