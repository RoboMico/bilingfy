using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bilingfy.Models;

public class FilterOptions : INotifyPropertyChanged
{
    private string _searchText = "";
    private SortType _sorting = SortType.Default;
    private bool _isSortingReversed = false;
    private bool _onlyShowUntranslated = false;

    /// <summary>
    /// The type of sorting for the entries.
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// The default sorting, in the order of which the entries were added.
        /// </summary>
        Default,

        /// <summary>
        /// Sort by the key in alphabetical order.
        /// </summary>
        Key,

        /// <summary>
        /// Sort by the reference text in alphabetical order.
        /// </summary>
        Reference,

        /// <summary>
        /// Sort by the target text in alphabetical order.
        /// </summary>
        Text
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value) return;
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public SortType Sorting
    {
        get => _sorting;
        set
        {
            if (_sorting == value) return;
            _sorting = value;
            OnPropertyChanged();
        }
    }

    public bool IsSortingReversed
    {
        get => _isSortingReversed;
        set
        {
            if (_isSortingReversed == value) return;
            _isSortingReversed = value;
            OnPropertyChanged();
        }
    }

    public bool OnlyShowUntranslated
    {
        get => _onlyShowUntranslated;
        set
        {
            if (_onlyShowUntranslated == value) return;
            _onlyShowUntranslated = value;
            OnPropertyChanged();
        }
    }

    public static Comparison<Entry> GetSortComparison(SortType sortType, bool reversed = false)
    {
        Comparison<Entry> comp = sortType switch
        {
            SortType.Key => (x, y) => string.Compare(x.Key, y.Key, StringComparison.Ordinal),
            SortType.Reference => (x, y) => string.Compare(x.Ref ?? x.Key, y.Ref ?? y.Key, StringComparison.Ordinal),
            SortType.Text => (x, y) => string.Compare(x.Value, y.Value, StringComparison.Ordinal),
            _ => (x, y) => x.SortOrder.CompareTo(y.SortOrder),
        };
        if (reversed)
        {
            return (x, y) => -comp(x, y);
        }
        else
        {
            return comp;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}