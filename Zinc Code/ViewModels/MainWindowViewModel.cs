using Avalonia;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Zinc_Code.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IStorageProvider _storageProvider;

        [ObservableProperty]
        private ObservableCollection<EditorViewModel> _documents = new();

        [ObservableProperty]
        private EditorViewModel? _selectedDocument;

        public MainWindowViewModel(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
            NewDocument();
        }

        [RelayCommand]
        private void NewDocument()
        {
            var doc = new EditorViewModel();
            Documents.Add(doc);
            SelectedDocument = doc;
        }

        [RelayCommand]
        public async Task ReadCode()
        {
            try
            {
                var filePicker = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "选择代码文件",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("代码文件")
                        {
                            Patterns = new[] { "*.cpp", "*.c", "*.h", "*.hpp" },
                            MimeTypes = new[] { "text/plain" }
                        }
                    }
                });

                if (filePicker == null || filePicker.Count == 0) return;

                var selected = filePicker[0];
                await using var readStream = await selected.OpenReadAsync();
                using var reader = new StreamReader(readStream);
                var content = await reader.ReadToEndAsync();

                var doc = new EditorViewModel(selected.Name, selected.Path.AbsolutePath, content);
                Documents.Add(doc);
                SelectedDocument = doc;
            }
            catch (Exception ex)
            {
                var doc = new EditorViewModel("错误", "", $"读取文件失败: {ex.Message}");
                Documents.Add(doc);
                SelectedDocument = doc;
            }
        }

        [RelayCommand]
        public async Task SaveCode()
        {
            if (SelectedDocument == null) return;

            try
            {
                if (string.IsNullOrEmpty(SelectedDocument.Filename))
                {
                    var filePicker = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "选择代码文件",
                        AllowMultiple = false,
                        FileTypeFilter = new[]
                        {
                            new FilePickerFileType("代码文件")
                            {
                                Patterns = new[] { "*.cpp", "*.c", "*.h", "*.hpp" },
                                MimeTypes = new[] { "text/plain" }
                            }
                        }
                    });

                    if (filePicker == null || filePicker.Count == 0) return;

                    var selected = filePicker[0];
                    await using var writeStream = await selected.OpenWriteAsync();
                    using var writer = new StreamWriter(writeStream);
                    await writer.WriteAsync(SelectedDocument.Document.Text);

                    SelectedDocument.Filename = selected.Name;
                    SelectedDocument.Filepath = selected.Path.AbsolutePath;
                }
                else
                {
                    await using var writer = new StreamWriter(SelectedDocument.Filepath);
                    await writer.WriteAsync(SelectedDocument.Document.Text);
                }
            }
            catch (Exception ex)
            {
                var doc = new EditorViewModel("错误", "", $"保存文件失败: {ex.Message}");
                Documents.Add(doc);
                SelectedDocument = doc;
            }
        }

        [RelayCommand]
        private void CloseDocument(EditorViewModel doc)
        {
            if (doc == null) return;

            if (Documents.Count <= 1)
            {
                NewDocument();
            }

            if (Documents.Remove(doc))
            {
                if (ReferenceEquals(SelectedDocument, doc))
                {
                    SelectedDocument = Documents.Count > 0 ? Documents[^1] : null;
                }
            }
        }

        [RelayCommand]
        private static void Exit()
        {
            Environment.Exit(0);
        }
    }
}