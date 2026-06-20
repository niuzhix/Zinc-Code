using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Zinc_Code.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IStorageProvider _storageProvider;

        [ObservableProperty]
        private TextDocument _document = new TextDocument();

        [ObservableProperty]
        private string _filename = string.Empty;

        public MainWindowViewModel(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
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

                if (filePicker==null || filePicker.Count == 0)
                {
                    return;
                }
                var selected = filePicker[0];

                Filename = selected.Name;

                await using var readStream = await selected.OpenReadAsync();
                using var reader = new StreamReader(readStream);
                Document = new TextDocument(await reader.ReadToEndAsync());
            }
            catch (Exception ex)
            {
                Document = new TextDocument($"读取文件失败: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task SaveCode()
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

                if (filePicker == null || filePicker.Count == 0)
                {
                    return;
                }
                var selected = filePicker[0];

                await using var writeStream = await selected.OpenWriteAsync();
                using var writer = new StreamWriter(writeStream);
                await writer.WriteAsync(Document.Text);
            }
            catch (Exception ex)
            {
                Document = new TextDocument($"读取文件失败: {ex.Message}");
            }
        }
    }
}
