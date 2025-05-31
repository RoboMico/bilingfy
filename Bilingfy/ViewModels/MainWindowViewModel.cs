using Bilingfy.Models;
using Bilingfy.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bilingfy.ViewModels;

/// <summary>
/// The delegate for the event handlers of the view model calling functions in the view.
/// </summary>
public delegate void ViewCallbackEventHandler();

/// <summary>
/// The data context model for the <see cref="MainWindow"/>.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        FilterOptions.PropertyChanged += (s, e) => ApplyFilter();
    }

    /// <summary>
    /// The collection of entries to be displayed.
    /// </summary>
    public ObservableCollection<Entry> Entries { get; set; } = [];

    /// <summary>
    /// The pool containing all the entries, including the ones that are not displayed.
    /// </summary>
    public List<Entry> EntryPool { get; set; } = [];

    /// <summary>
    /// Keys that are present in <see cref="EntryPool"/>.
    /// </summary>
    public HashSet<string> PresentKeys { get; set; } = [];

    /// <summary>
    /// The current filter options.
    /// </summary>
    public FilterOptions FilterOptions { get; set; } = new();

    /// <summary>
    /// Build the <see cref="EntryPool"/> based on the given reference and target dictionaries.
    /// </summary>
    /// <param name="refDict">The reference dictionary.</param>
    /// <param name="targetDict">The target dictionary.</param>
    public void BuildEntryPool(Dictionary<string, string> refDict, Dictionary<string, string> targetDict)
    {
        EntryPool.Clear();
        PresentKeys.Clear();
        Dictionary<string, Entry> tempDict = [];
        int counter = 0;
        foreach (var p in refDict)
        {
            tempDict[p.Key] = new Entry
            {
                Key = p.Key,
                Ref = p.Value,
                SortOrder = counter++
            };
        }
        foreach (var p in targetDict)
        {
            if (tempDict.TryGetValue(p.Key, out Entry? entry))
            {
                entry.Value = p.Value;
            }
            else
            {
                tempDict[p.Key] = new Entry
                {
                    Key = p.Key,
                    Ref = null,
                    Value = p.Value,
                    SortOrder = counter++
                };
            }
        }
        foreach (var p in tempDict)
        {
            EntryPool.Add(p.Value);
            PresentKeys.Add(p.Key);
        }
    }

    /// <summary>
    /// Update <see cref="Entries"/> based on the given filter options.
    /// </summary>
    /// <param name="options">The filter options. If null, <see cref="FilterOptions"/> is used.</param>
    public void ApplyFilter(FilterOptions? options = null)
    {
        options ??= FilterOptions;
        Entries.Clear();
        List<Entry> list = [];
        foreach (var entry in EntryPool)
        {
            if (options.OnlyShowUntranslated && string.IsNullOrEmpty(entry.Value))
            {
                continue;
            }
            if (!(
                string.IsNullOrEmpty(options.SearchText)
                || entry.Key.Contains(options.SearchText, StringComparison.OrdinalIgnoreCase)
                || (entry.Ref is not null && entry.Ref.Contains(options.SearchText, StringComparison.OrdinalIgnoreCase))
                || entry.Value.Contains(options.SearchText, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                continue;
            }
            list.Add(entry);
        }
        list.Sort(FilterOptions.GetSortComparison(options.Sorting, options.IsSortingReversed));
        foreach (var entry in list)
        {
            Entries.Add(entry);
        }
    }

    public Dictionary<string, string> ExportTarget()
    {
        Dictionary<string, string> result = [];
        foreach (var entry in EntryPool)
        {
            result[entry.Key] = entry.Value;
        }
        return result;
    }

    public bool AddEntry(string key)
    {
        if (PresentKeys.Contains(key))
        {
            return false;
        }
        PresentKeys.Add(key);
        EntryPool.Add(new Entry
        {
            Key = key,
            Ref = null,
            Value = "",
            SortOrder = EntryPool.Count
        });
        return true;
    }

    public void CommandOpenFile()
    {
        OpenFile?.Invoke();
    }

    public void CommandOpenReference()
    {
        OpenReference?.Invoke();
    }

    public void CommandSaveFile()
    {
        SaveFile?.Invoke();
    }

    public void CommandSaveAs()
    {
        SaveAs?.Invoke();
    }

    public void CommandSortButtonClicked()
    {
        FilterOptions.IsSortingReversed = !FilterOptions.IsSortingReversed;
    }

    public void CommandClearReferences()
    {
        ClearReference?.Invoke();
    }

    public event ViewCallbackEventHandler? OpenFile;

    public event ViewCallbackEventHandler? OpenReference;

    public event ViewCallbackEventHandler? SaveFile;

    public event ViewCallbackEventHandler? SaveAs;

    public event ViewCallbackEventHandler? ClearReference;
}