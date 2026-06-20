using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using Zinc_Code.Core.Abstractions;
using Zinc_Code.Core.Services;

namespace Zinc_Code.Views;

public partial class EditorView : UserControl
{
    private readonly ISettingsService _settings = SettingsService.Instance;

    public EditorView()
    {
        InitializeComponent();

        _settings.Preload();

        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);

        var textMateInstallation = Code.InstallTextMate(registryOptions);

        var languageId = registryOptions.GetLanguageByExtension(_settings.Read("CodeLanguage", ".cpp"))?.Id;
        if (languageId != null)
        {
            var scopeName = registryOptions.GetScopeByLanguageId(languageId);
            textMateInstallation.SetGrammar(scopeName);
        }
    }
}