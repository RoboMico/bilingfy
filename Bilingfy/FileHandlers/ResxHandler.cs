using System;
using System.Resources;
using System.Collections.Generic;
using System.Collections;

namespace Bilingfy.FileHandlers;

/// <summary>
/// Represents a Resx file handler.
/// </summary>
/// <param name="path">Path to the file.</param>
public class ResxHandler(string path) : FileHandler(path)
{
    public override Dictionary<string, string> Load()
    {
        ResourceReader reader = new(Path);
        Dictionary<string, string> dict = [];
        foreach (DictionaryEntry entry in reader)
        {
            string key = entry.Key.ToString() ?? throw new Exception();
            dict.Add(key, entry.Value?.ToString() ?? "");
        }
        return dict;
    }

    public override void Save(Dictionary<string, string> dict)
    {
        ResourceWriter writer = new(Path);
        foreach (KeyValuePair<string, string> entry in dict)
        {
            if (!string.IsNullOrEmpty(entry.Value))
            {
                writer.AddResource(entry.Key, entry.Value);
            }
        }
        writer.Generate();
    }
}