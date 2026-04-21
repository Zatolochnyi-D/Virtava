using System.Collections.Generic;
using Newtonsoft.Json;

namespace Virtava.Client
{
    public struct Vector
    {
        public float x;
        public float y;
        public float z;
    }

    public struct Difference
    {
        public string boneName;
        public Vector positionDifference;
        public Vector rotationDifference;
        public Vector scaleDifference;
    }

    public class ArkitBledshapeDifference
    {
        // private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        // {
            
        // };

        public static ArkitBledshapeDifference ReadFromJson(string json, Dictionary<string, string> namingConventionMap)
        {
            // TODO: Handle different jsons provided.
            var obj = JsonConvert.DeserializeObject<Dictionary<string, Difference[]>>(json);
            var result = new ArkitBledshapeDifference();
            foreach (var (key, value) in obj!)
                result.Differences[ArkitBlendshapes.GetBlendshape(key, namingConventionMap)] = value;
            return result;
        }

        public Dictionary<ArkitBlendshape, Difference[]> Differences = new Dictionary<ArkitBlendshape, Difference[]>();
    }
}