using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;

namespace Zinc_Code.ViewModels
{
    public partial class EditorViewModel : ViewModelBase
    {
        private TextDocument _document = new TextDocument();

        [ObservableProperty]
        private bool _isModified;

        public TextDocument Document
        {
            get => _document;
            set
            {
                if (_document != null)
                {
                    _document.TextChanged -= OnDocumentTextChanged;
                }

                SetProperty(ref _document, value);

                if (_document != null)
                {
                    _document.TextChanged += OnDocumentTextChanged;
                }
            }
        }

        private string _filename = string.Empty;
        public string Filename
        {
            get => _filename;
            set => SetProperty(ref _filename, value);
        }

        private string _filepath = string.Empty;
        public string Filepath
        {
            get => _filepath;
            set => SetProperty(ref _filepath, value);
        }

        public EditorViewModel()
        {
            Document.TextChanged += OnDocumentTextChanged;
            Filename = "未标题";
        }

        public EditorViewModel(int count) : this()
        {
            Filename = $"未标题{count}.cpp";
            IsModified = false;
        }

        public EditorViewModel(string filename, string filepath, string content) : this()
        {
            Filename = filename;
            Filepath = filepath;
            Document = new TextDocument(content);
            IsModified = false;
        }
        private void OnDocumentTextChanged(object? sender, EventArgs e)
        {
            IsModified = true;
            UpdateTitle();
        }

        [RelayCommand]
        private void UpdateTitle()
        {
            var cleanName = Filename.Replace("*", "");

            var nameWithoutExt = Path.GetFileNameWithoutExtension(cleanName);
            var ext = Path.GetExtension(cleanName);

            Filename = IsModified ? $"{nameWithoutExt}*{ext}" : $"{nameWithoutExt}{ext}";
        }
    }
}