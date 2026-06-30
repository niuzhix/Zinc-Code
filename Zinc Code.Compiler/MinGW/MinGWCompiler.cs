using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zinc_Code.Compiler.Interfaces;
using Zinc_Code.Compiler.Models;

namespace Zinc_Code.Compiler.MinGW;

public class MinGWCompiler : ICppCompiler
{
    private readonly string _gppPath;

    public bool IsAvailable => File.Exists(_gppPath);

    public MinGWCompiler(string mingwPath)
    {
        if (string.IsNullOrWhiteSpace(mingwPath))
            throw new ArgumentException("MinGW路径不能为空", nameof(mingwPath));

        _gppPath = Path.Combine(mingwPath, "bin", "g++.exe");
    }

    public async Task<CompilationResult> CompileAsync(
        CompilationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.SourceCode))
                return CompilationResult.Failure("源代码为空");

            if (!IsAvailable)
                return CompilationResult.Failure($"编译器不存在: {_gppPath}");

            string tempDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ZincCode",
                "Builds",
                Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(tempDir);

            string sourceFile = Path.Combine(tempDir, "source.cpp");
            string outputExe = Path.Combine(tempDir, "program.exe");

            await File.WriteAllTextAsync(sourceFile, request.SourceCode, cancellationToken);

            string args = BuildArguments(sourceFile, outputExe, request.Options);

            var startInfo = new ProcessStartInfo
            {
                FileName = _gppPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = tempDir
            };

            using var process = new Process { StartInfo = startInfo };
            var output = new StringBuilder();
            var error = new StringBuilder();

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null) output.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null) error.AppendLine(e.Data);
            };

            var sw = Stopwatch.StartNew();
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            int timeoutMs = request.Options.TimeoutSeconds * 1000;
            var exitTask = process.WaitForExitAsync(cancellationToken);
            var timeoutTask = Task.Delay(timeoutMs, cancellationToken);

            var completedTask = await Task.WhenAny(exitTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                try { process.Kill(); } catch { }
                sw.Stop();
                return CompilationResult.Failure($"编译超时（{request.Options.TimeoutSeconds}秒）", sw.Elapsed);
            }

            await exitTask;
            sw.Stop();

            bool exeExists = File.Exists(outputExe);

            if (process.ExitCode == 0 && exeExists)
            {
                return CompilationResult.Successful(outputExe, sw.Elapsed, output.ToString());
            }
            else
            {
                string allOutput = error.Length > 0 ? error.ToString() : output.ToString();
                return CompilationResult.Failure($"编译失败\n{allOutput}", sw.Elapsed);
            }
        }
        catch (Exception ex)
        {
            return CompilationResult.Failure($"编译异常: {ex.Message}");
        }
    }

    private string BuildArguments(string sourceFile, string outputExe, CompilerOptions options)
    {
        var args = new List<string>
        {
            $"\"{sourceFile}\"",
            $"-o \"{outputExe}\""
        };

        if (!string.IsNullOrEmpty(options.OptimizationLevel))
            args.Add($"-{options.OptimizationLevel}");

        if (!string.IsNullOrEmpty(options.LanguageStandard))
            args.Add($"-std={options.LanguageStandard}");

        if (options.StaticLink)
            args.Add("-static");

        if (options.EnableWarnings)
        {
            args.Add("-Wall");
            args.Add("-Wextra");
        }

        foreach (var kv in options.AdditionalArguments)
        {
            args.Add($"{kv.Key} {kv.Value}");
        }

        return string.Join(" ", args);
    }
}