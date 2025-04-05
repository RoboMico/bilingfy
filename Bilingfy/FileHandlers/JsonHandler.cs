using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Bilingfy.FileHandlers;

/// <summary>
/// Represents a JSON file handler.
/// </summary>
/// <param name="path">Path to the file.</param>
public class JsonHandler(string path) : FileHandler(path)
{
    public override Dictionary<string, string> Load()
    {
        string json = File.ReadAllText(Path);
        return JsonSerializer.Deserialize(
            json, typeof(Dictionary<string, string>), JsonGenerationContext.Default) as Dictionary<string, string>
            ?? [];
    }

    public override void Save(Dictionary<string, string> dict)
    {
        Dictionary<string, string> tempDict = [];
        foreach (var pair in dict)
        {
            if (!string.IsNullOrEmpty(pair.Value))
            {
                tempDict.Add(pair.Key, pair.Value);
            }
        }
        string json = JsonSerializer.Serialize(
            tempDict, typeof(Dictionary<string, string>), JsonGenerationContext.Default);
        File.WriteAllText(Path, json);
    }
}