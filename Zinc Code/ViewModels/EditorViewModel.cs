using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading.Tasks;
using Zinc_Code.Compiler;
using Zinc_Code.Compiler.Interfaces;
using Zinc_Code.Compiler.Models;

namespace Zinc_Code.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    private TextDocument _document = new TextDocument();

    [ObservableProperty]
    private bool _isModified;

    [ObservableProperty]
    private string _output = string.Empty;

    [ObservableProperty]
    private bool _isCompiling;

    [ObservableProperty]
    private string _compilationStatus = "就绪";

    private readonly ICppCompiler? _compiler;

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

    // 无参构造函数（用于设计时）
    public EditorViewModel()
    {
        Document.TextChanged += OnDocumentTextChanged;
        Filename = "未标题";
        _compiler = null;
    }

    // 带编译器的构造函数
    public EditorViewModel(ICppCompiler compiler) : this()
    {
        _compiler = compiler;
    }

    public EditorViewModel(int count) : this()
    {
        Filename = $"未标题{count}.cpp";
        IsModified = false;
    }

    public EditorViewModel(int count, ICppCompiler compiler) : this()
    {
        Filename = $"未标题{count}.cpp";
        IsModified = false;
        _compiler = compiler;
    }

    public EditorViewModel(string filename, string filepath, string content) : this()
    {
        Filename = filename;
        Filepath = filepath;
        Document = new TextDocument(content);
        IsModified = false;
        _compiler = null;
    }

    public EditorViewModel(string filename, string filepath, string content, ICppCompiler compiler) : this()
    {
        Filename = filename;
        Filepath = filepath;
        Document = new TextDocument(content);
        IsModified = false;
        _compiler = compiler;
    }

    private void OnDocumentTextChanged(object? sender, EventArgs e)
    {
        IsModified = true;
        UpdateTitle();
    }

    [RelayCommand]
    public void UpdateTitle()
    {
        var cleanName = Filename.Replace("*", "");

        var nameWithoutExt = Path.GetFileNameWithoutExtension(cleanName);
        var ext = Path.GetExtension(cleanName);

        Filename = IsModified ? $"{nameWithoutExt}*{ext}" : $"{nameWithoutExt}{ext}";
    }

    [RelayCommand]
    private async Task CompileAsync()
    {
        if (_compiler == null)
        {
            Output = "错误: 编译器服务未初始化";
            return;
        }

        if (IsCompiling)
            return;

        string sourceCode = Document.Text;
        if (string.IsNullOrWhiteSpace(sourceCode))
        {
            Output = "错误: 源代码为空，无法编译";
            return;
        }

        IsCompiling = true;
        CompilationStatus = "编译中...";
        Output = "正在编译...";

        try
        {
            var request = new CompilationRequest
            {
                SourceCode = sourceCode,
                Options = new CompilerOptions
                {
                    OptimizationLevel = "O2",
                    LanguageStandard = "c++17",
                    StaticLink = true,
                    EnableWarnings = true,
                    TimeoutSeconds = 10
                }
            };

            var result = await _compiler.CompileAsync(request);

            if (result.Success)
            {
                CompilationStatus = "编译成功";
                Output = $"编译成功\n" +
                         $"耗时: {result.Duration.TotalMilliseconds:F1}ms\n" +
                         $"输出文件: {result.OutputPath}";

                if (!string.IsNullOrWhiteSpace(result.StandardOutput))
                {
                    Output += $"\n\n编译器信息:\n{result.StandardOutput}";
                }

                if (result.OutputPath != null && File.Exists(result.OutputPath))
                {
                    await RunCompiledProgramAsync(result.OutputPath);
                }
            }
            else
            {
                CompilationStatus = "编译失败";
                Output = $"编译失败\n{result.ErrorOutput}";
            }
        }
        catch (Exception ex)
        {
            CompilationStatus = "异常";
            Output = $"编译异常: {ex.Message}";
        }
        finally
        {
            IsCompiling = false;
        }
    }

    private async Task RunCompiledProgramAsync(string exePath)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = exePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            if (process != null)
            {
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (!string.IsNullOrEmpty(output))
                    Output += $"\n\n程序输出:\n{output}";
                if (!string.IsNullOrEmpty(error))
                    Output += $"\n\n程序错误:\n{error}";
                if (process.ExitCode != 0)
                    Output += $"\n\n程序退出码: {process.ExitCode}";
            }
        }
        catch (Exception ex)
        {
            Output += $"\n\n运行失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearOutput()
    {
        Output = string.Empty;
        CompilationStatus = "就绪";
    }
}