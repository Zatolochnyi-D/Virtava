namespace ArkitBlendshapesNameMapBuilder;

public class SnakeCaseJoiner : NamingCaseJoiner
{
    public override string Join(IEnumerable<string> nameParts)
    {
        return string.Join("_", nameParts);
    }
}