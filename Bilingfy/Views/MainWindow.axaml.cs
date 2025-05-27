using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Bilingfy.FileHandlers;
using Bilingfy.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace Bilingfy.Views;

/// <summary>
/// The main window.
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _vm;

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

    private TopLevel? GetFileSystem()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null)
        {
            MsgBox.ShowUnknownError(this, "TopLevel");
        }
        return topLevel;
    }

    private async void OpenReference()
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
            _sourceDict = FileHandler.Recognize(file.Path.AbsolutePath).Load();
        }
        catch (Exception ex)
        {
            MsgBox.ShowError(this, "Error", string.Format(@"Failed to load the file:
{0}

Perhaps the file is corrupted or in a wrong format.", ex.Message));
            return;
        }
        var targetDict = _vm.ExportTarget();
        _vm.BuildEntryPool(_sourceDict, targetDict);
        _vm.ApplyFilter();
    }

    private async void OpenFile()
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
        }
        catch (Exception ex)
        {
            MsgBox.ShowError(this, "Error", string.Format(@"Failed to load the file:
{0}

Perhaps the file is corrupted or in a wrong format.", ex.Message));
            return;
        }
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
        try
        {
            handler.Save(_vm.ExportTarget());
        }
        catch (Exception ex)
        {
            MsgBox.ShowError(this, "Error", string.Format(@"Failed to save the file:
{0}", ex.Message));
        }
    }

    private async void SaveAs()
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
        SaveFile();
    }
}