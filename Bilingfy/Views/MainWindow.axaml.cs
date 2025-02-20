using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Bilingfy.Models;
using Bilingfy.ViewModels;

namespace Bilingfy.Views;

/// <summary>
/// The main window.
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowViewModel _vm;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        _vm = new MainWindowViewModel();
        DataContext = _vm;
        InitializeComponent();
    }

    private async void MenuOpenReference(object sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open JSON file",
            AllowMultiple = false,
            FileTypeFilter = [new("JSON file")
            {
                Patterns = ["*.json"],
                AppleUniformTypeIdentifiers = ["public.json"],
                MimeTypes = ["application/json"]
            }]
        });

        if (files.Count == 0) return;
        var file = files[0];
        StreamReader reader = new(await file.OpenReadAsync());
        var json = reader.ReadToEnd();
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (dict is null) return;
        Dictionary<string, string> targetDict = _vm.ExportTarget();
        _vm.BuildEntryPool(dict, targetDict);
        _vm.ApplyFilter();
    }
}