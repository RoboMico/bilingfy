using System;
using System.Collections.Generic;

namespace Bilingfy.FileHandlers;

/// <summary>
/// Base class for file handlers.
/// </summary>
public abstract class FileHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileHandler{T}"/> class.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="options">Options for the file handler.</param>
    public FileHandler(string path)
    {
        Path = path;
    }

    /// <summary>
    /// Path to the file.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Saves the dictionary to the file.
    /// </summary>
    /// <param name="dict">Dictionary to save.</param>
    public abstract void Save(Dictionary<string, string> dict);

    /// <summary>
    /// Loads the dictionary from the file.
    /// </summary>
    /// <returns>Dictionary loaded from the file.</returns>
    public abstract Dictionary<string, string> Load();

    /// <summary>
    /// Recognizes the file extension and returns the corresponding file handler.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>The corresponding file handler.</returns>
    /// <exception cref="NotSupportedException">Thrown if the file extension is not supported.</exception>
    public static FileHandler Recognize(string path)
    {
        string extension = System.IO.Path.GetExtension(path);
        return extension switch
        {
            ".json" => new JsonHandler(path),
            ".lang" => new LangPropHandler(path),
            ".resx" => new ResxHandler(path),
            _ => throw new NotSupportedException($"Extension '{extension}' is not supported."),
        };
    }
}