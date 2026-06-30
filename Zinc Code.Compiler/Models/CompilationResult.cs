using System;

namespace Zinc_Code.Compiler.Models;

public class CompilationResult
{
    public bool Success { get; set; }
    public string? OutputPath { get; set; }
    public string StandardOutput { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public CompilerError[] Errors { get; set; } = Array.Empty<CompilerError>();

    public static CompilationResult Successful(string outputPath, TimeSpan duration, string stdOutput = "")
    {
        return new CompilationResult
        {
            Success = true,
            OutputPath = outputPath,
            StandardOutput = stdOutput,
            Duration = duration
        };
    }

    public static CompilationResult Failure(string error, TimeSpan duration = default)
    {
        return new CompilationResult
        {
            Success = false,
            ErrorOutput = error,
            Duration = duration
        };
    }
}

public class CompilerError
{
    public string? File { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public string Message { get; set; } = string.Empty;
    public CompilerErrorSeverity Severity { get; set; }
}

public enum CompilerErrorSeverity
{
    Info,
    Warning,
    Error,
    Fatal
}