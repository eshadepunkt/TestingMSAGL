using System.Collections.Generic;

namespace TestingMSAGL.DataLinker.RoutedOperation

{
    public record NamedOperation(int Id, string Name, IEnumerable<string> dataSet);
}