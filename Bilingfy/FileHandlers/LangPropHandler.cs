using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Bilingfy.FileHandlers;

/// <summary>
/// Represents a language properties (.lang) file handler.
/// </summary>
/// <param name="path">Path to the file.</param>
public class LangPropHandler(string path) : FileHandler(path)
{
    public override Dictionary<string, string> Load()
    {
        Dictionary<string, string> result = [];
        using var inputStream = File.OpenRead(Path);
        using var reader = new StreamReader(inputStream);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith('#') || line.StartsWith('!'))
                continue;

            int separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0) continue;

            string key = line[..separatorIndex].Trim();
            string value = line[(separatorIndex + 1)..].Trim();
            result[key] = Regex.Unescape(value);
        }
        return result;
    }

    public override void Save(Dictionary<string, string> dict)
    {
        using var outputStream = File.OpenWrite(Path);
        using var writer = new StreamWriter(outputStream);
        foreach (var entry in dict)
        {
            if (!string.IsNullOrEmpty(entry.Value))
            {
                writer.WriteLine($"{entry.Key}={Regex.Escape(entry.Value)}");
            }
        }
    }
}