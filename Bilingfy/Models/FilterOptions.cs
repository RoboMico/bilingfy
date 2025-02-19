using System;

namespace Bilingfy.Models;

public class FilterOptions
{
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
        /// Sort by the key in alphabetical order (A-Z).
        /// </summary>
        Key,

        /// <summary>
        /// Sort by the key in reverse alphabetical order (Z-A).
        /// </summary>
        KeyReverse,

        /// <summary>
        /// Sort by the source text in alphabetical order (A-Z).
        /// </summary>
        Source,

        /// <summary>
        /// Sort by the source text in reverse alphabetical order (Z-A).
        /// </summary>
        SourceReverse,

        /// <summary>
        /// Sort by the target text in alphabetical order (A-Z).
        /// </summary>
        Target,

        /// <summary>
        /// Sort by the target text in reverse alphabetical order (Z-A).
        /// </summary>
        TargetReverse
    }

    public string SearchText { get; set; } = "";

    public SortType Sorting { get; set; } = SortType.Default;

    public bool OnlyShowUntranslated { get; set; } = false;

    public static Comparison<Entry> GetSortComparison(SortType sortType)
    {
        return sortType switch
        {
            SortType.Key => (x, y) => string.Compare(x.Key, y.Key, StringComparison.Ordinal),
            SortType.KeyReverse => (x, y) => string.Compare(y.Key, x.Key, StringComparison.Ordinal),
            SortType.Source => (x, y) => string.Compare(x.Source ?? "", y.Source ?? "", StringComparison.Ordinal),
            SortType.SourceReverse => (x, y) => string.Compare(y.Source ?? "", x.Source ?? "", StringComparison.Ordinal),
            SortType.Target => (x, y) => string.Compare(x.Target, y.Target, StringComparison.Ordinal),
            SortType.TargetReverse => (x, y) => string.Compare(y.Target, x.Target, StringComparison.Ordinal),
            _ => (x, y) => x.SortOrder.CompareTo(y.SortOrder),
        };
    }
}