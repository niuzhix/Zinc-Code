using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using Zinc_Code.Core.Abstractions;
using Zinc_Code.Core.Services;

namespace Zinc_Code.Views;

public partial class MainWindow : Window
{
    private readonly ISettingsService _settings = SettingsService.Instance;

    public MainWindow()
    {
        InitializeComponent();

        _settings.Preload();

        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);

        // 安装 TextMate 到编辑器
        var textMateInstallation = Code.InstallTextMate(registryOptions);

        // 根据文件扩展名设置语法（.cpp, .h, .hpp）
        // RegistryOptions 内置了所有 VSCode 支持的语言[citation:3]
        var languageId = registryOptions.GetLanguageByExtension(_settings.Read("CodeLanguage",".cpp"))?.Id;
        if (languageId != null)
        {
            var scopeName = registryOptions.GetScopeByLanguageId(languageId);
            textMateInstallation.SetGrammar(scopeName);
        }


    }
}