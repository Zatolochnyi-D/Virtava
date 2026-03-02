namespace ArkitBlendshapesNameMapBuilder;

public class PascalCaseJoiner : NamingCaseJoiner
{
    public override string Join(IEnumerable<string> nameParts)
    {
        return string.Join("", nameParts.Select(x => $"{char.ToUpper(x[0])}{x[1..]}"));
    }
}
