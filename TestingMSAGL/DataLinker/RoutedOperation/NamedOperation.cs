using System.Collections.Generic;

namespace TestingMSAGL.DataStructure.RoutedOperation

{
    public record NamedOperation(int Id, string Name, IEnumerable<string> dataSet);
}