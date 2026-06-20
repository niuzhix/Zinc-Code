using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Zinc_Code.ViewModels
{
    public partial class EditorViewModel : ViewModelBase
    {
        private TextDocument _document = new TextDocument();
        public TextDocument Document
        {
            get => _document;
            set => SetProperty(ref _document, value);
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

        public EditorViewModel() { }

        public EditorViewModel(string filename, string filepath, string content)
        {
            Filename = filename;
            Filepath = filepath;
            Document = new TextDocument(content);
        }
    }
}