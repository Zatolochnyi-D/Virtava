namespace ArkitBlendshapesNameMapBuilder;

public class CamelCaseParser : NamingCaseParser
{
    public override IEnumerable<string> Parse(string name)
    {
        var capitalLetterIndexes = name.Index().Where(x => char.IsUpper(x.Item)).Select(x => x.Index);

        var ranges = new List<(Index, Index)>();
        var previousIndex = 0;
        foreach (var index in capitalLetterIndexes)
        {
            ranges.Add((new Index(previousIndex), new Index(index)));
            previousIndex = index;
        }
        ranges.Add((new Index(previousIndex), ^0));

        var parts = new List<string>();
        foreach (var (begin, end) in ranges)
            parts.Add(name[begin..end].ToLower());

        return parts;
    }
}
