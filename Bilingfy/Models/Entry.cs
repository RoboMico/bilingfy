using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bilingfy.Models;

/// <summary>
/// Represents an entry in the language file.
/// </summary>
public class Entry : INotifyPropertyChanged
{
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
    /// The text in the target language.
    /// </summary>
    public string Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// The order of the entry in which it is imported.
    /// </summary>
    public int SortOrder { init; get; } = 0;

    /// <summary>
    /// Whether the entry has a reference in the source language.
    /// </summary>
    public bool HasReference => Ref is not null;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
