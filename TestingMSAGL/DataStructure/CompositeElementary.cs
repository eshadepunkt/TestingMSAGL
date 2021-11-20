namespace TestingMSAGL.DataStructure
{
    public class CompositeElementary : Composite
    {
        public CompositeElementary()
        {
            Type = "elementary";
        }
        public string SomeElementaryAttribute { get; set; }
    }
}