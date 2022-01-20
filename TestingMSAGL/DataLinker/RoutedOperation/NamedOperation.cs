using System.Collections.Generic;

namespace ComplexEditor.DataLinker.RoutedOperation

{
    public record NamedOperation(int Id, string Name, IEnumerable<string> dataSet);
}