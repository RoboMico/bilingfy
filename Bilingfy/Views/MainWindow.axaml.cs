using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Bilingfy.FileHandlers;
using Bilingfy.Models;
using Bilingfy.ViewModels;

namespace Bilingfy.Views;

/// <summary>
/// The main window.
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowViewModel _vm;

    private Dictionary<string, string> _sourceDict = [];

    private string? _saveFilePath = null;

    public readonly List<FilePickerFileType> SupportedFileTypes =
    [
        new("All supported files")
        {
            Patterns = ["*.json", "*.lang", "*.resx"]
        },
        new("JSON file")
        {
            Patterns = ["*.json"]
        },
        new(".lang properties file")
        {
            Patterns = ["*.lang"]
        },
        new("ResX resource file")
        {
            Patterns = ["*.resx"]
        }
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        _vm = new MainWindowViewModel();
        _vm.OpenReference += OpenReference;
        _vm.OpenFile += OpenFile;
        _vm.SaveFile += SaveFile;
        _vm.SaveAs += SaveAs;
        DataContext = _vm;
        InitializeComponent();
    }

    private async void OpenReference()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Language File as the Reference",
            AllowMultiple = false,
            FileTypeFilter = SupportedFileTypes
        });

        if (files.Count == 0) return;
        var file = files[0];
        _sourceDict = FileHandler.Recognize(file.Path.AbsolutePath).Load();
        var targetDict = _vm.ExportTarget();
        _vm.BuildEntryPool(_sourceDict, targetDict);
        _vm.ApplyFilter();
    }

    private async void OpenFile()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Language File",
            AllowMultiple = false,
            FileTypeFilter = SupportedFileTypes
        });

        if (files.Count == 0) return;
        var file = files[0];
        var targetDict = FileHandler.Recognize(file.Path.AbsolutePath).Load();
        _vm.BuildEntryPool(_sourceDict, targetDict);
        _vm.ApplyFilter();
    }

    private void SaveFile()
    {
        if (_saveFilePath == null)
        {
            SaveAs();
            return;
        }
        var handler = FileHandler.Recognize(_saveFilePath);
        handler.Save(_vm.ExportTarget());
    }

    private async void SaveAs()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save File",
            FileTypeChoices = SupportedFileTypes[1..]
        });
        if (file is null) return;
        _saveFilePath = file.Path.AbsolutePath;
        SaveFile();
    }
}