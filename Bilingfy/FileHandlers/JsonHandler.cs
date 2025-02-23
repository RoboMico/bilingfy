using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? [];
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
        string json = JsonSerializer.Serialize(tempDict, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path, json);
    }
}