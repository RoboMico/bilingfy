using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Bilingfy.FileHandlers;
using Bilingfy.ViewModels;
using System;
using System.Collections.Generic;

namespace Bilingfy.Views;

/// <summary>
/// The main window.
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _vm;

    private Dictionary<string, string> _refDict = [];

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
        DataContext = _vm;
        InitializeComponent();
    }

    private TopLevel? GetFileSystem()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null)
        {
            MsgBox.ShowUnknownError(this, "TopLevel");
        }
        return topLevel;
    }

    private async void OnOpenReference(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetFileSystem();
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Language File as the Reference",
            AllowMultiple = false,
            FileTypeFilter = SupportedFileTypes
        });

        if (files.Count == 0) return;
        var file = files[0];
        try
        {
            _refDict = FileHandler.Recognize(file.Path.AbsolutePath).Load();
        }
        catch (Exception ex)
        {
            MsgBox.ShowError(this, "Error", string.Format(@"Failed to load the file:
{0}

Perhaps the file is corrupted or in a wrong format.", ex.Message));
            return;
        }
        var targetDict = _vm.ExportTarget();
        _vm.BuildEntryPool(_refDict, targetDict);
        _vm.ApplyFilter();
    }

    private async void OnOpenFile(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetFileSystem();
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Language File",
            AllowMultiple = false,
            FileTypeFilter = SupportedFileTypes
        });

        if (files.Count == 0) return;
        var file = files[0];
        Dictionary<string, string> targetDict;
        try
        {
            targetDict = FileHandler.Recognize(file.Path.AbsolutePath).Load();
            _saveFilePath = file.Path.AbsolutePath;
        }
        catch (Exception ex)
        {
            MsgBox.ShowError(this, "Error", string.Format(@"Failed to load the file:
{0}

Perhaps the file is corrupted or in a wrong format.", ex.Message));
            return;
        }
        _vm.BuildEntryPool(_refDict, targetDict);
        _vm.ApplyFilter();
        _vm.IsUnsaved = false;
    }

    private void OnSaveFile(object? sender, RoutedEventArgs e)
    {
        if (_saveFilePath is null)
        {
            OnSaveAs(sender, e);
            return;
        }
        var handler = FileHandler.Recognize(_saveFilePath);
        try
        {
            handler.Save(_vm.ExportTarget());
            _vm.IsUnsaved = false;
        }
        catch (Exception ex)
        {
            MsgBox.ShowError(this, "Error", string.Format(@"Failed to save the file:
{0}", ex.Message));
        }
    }

    private async void OnSaveAs(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetFileSystem();
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save File",
            FileTypeChoices = SupportedFileTypes[1..]
        });

        if (file is null) return;
        _saveFilePath = file.Path.AbsolutePath;
        OnSaveFile(sender, e);
    }

    private async void OnClearReference(object? sender, RoutedEventArgs e)
    {
        var res = await MsgBox.ShowConfirmation(this, "Clear Reference", "Are you sure you want to clear the references of all entries?");
        if (!res) return;
        _refDict.Clear();
        _vm.BuildEntryPool(_refDict, _vm.ExportTarget());
        _vm.ApplyFilter();
    }

    private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!_vm.IsUnsaved)
        {
            return;
        }
        e.Cancel = true;
        var res = await MsgBox.ShowConfirmation(this, "Exit", "Exit without saving?");
        if (res)
        {
            Environment.Exit(0);
        }
    }
}