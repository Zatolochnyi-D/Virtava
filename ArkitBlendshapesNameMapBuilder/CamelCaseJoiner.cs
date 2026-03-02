namespace ArkitBlendshapesNameMapBuilder;

public class CamelCaseJoiner : NamingCaseJoiner
{
    public override string Join(IEnumerable<string> nameParts)
    {
        return $"{nameParts.First()}{string.Join("", nameParts.Skip(1).Select(x => $"{char.ToUpper(x[0])}{x[1..]}"))}";
    }
}
