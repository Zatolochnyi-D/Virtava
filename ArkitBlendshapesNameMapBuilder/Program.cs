using System.Text.Json;

namespace ArkitBlendshapesNameMapBuilder;

public class Program
{
    static void Main(string[] args)
    {
        var defaultNames = File.ReadAllLines("../../../defaultBlendshapeNames.txt");
        var defaultParser = new CamelCaseParser();
        IEnumerable<NamingCaseJoiner> joiners = [new CamelCaseJoiner(), new PascalCaseJoiner(), new SnakeCaseJoiner()];
        var dictionary = new Dictionary<string, string>();
        foreach (var name in defaultNames)
        {
            var parts = defaultParser.Parse(name);
            foreach (var joiner in joiners)
            {
                dictionary[joiner.Join(parts)] = name;
            }
        }
        File.WriteAllText("../../../blendshapeNamesMap.json", JsonSerializer.Serialize(dictionary, new JsonSerializerOptions() { WriteIndented = true }));
    }
}