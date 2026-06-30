using System.Collections.Generic;

namespace Zinc_Code.Compiler.Models;

public class CompilationRequest
{
    public string SourceCode { get; set; } = string.Empty;
    public CompilerOptions Options { get; set; } = new();
    public string? WorkingDirectory { get; set; }
    public Dictionary<string, string> AdditionalFiles { get; set; } = new();
}