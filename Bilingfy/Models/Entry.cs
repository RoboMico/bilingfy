using CommunityToolkit.Mvvm.ComponentModel;

namespace Bilingfy.Models;

/// <summary>
/// Represents an entry in the language file.
/// </summary>
public partial class Entry : ObservableObject
{
    [ObservableProperty]
    private string _value = "";

    /// <summary>
    /// The index key of the entry.
    /// </summary>
    public string Key { init; get; } = "";

    /// <summary>
    /// The text in the source language.
    /// </summary>
    public string? Ref { init; get; } = null;

    /// <summary>
    /// The order of the entry in which it is imported.
    /// </summary>
    public int SortOrder { init; get; } = 0;

    /// <summary>
    /// Whether the entry has a reference in the source language.
    /// </summary>
    public bool HasReference => Ref is not null;
}
