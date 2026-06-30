using System.Collections.Generic;

namespace Zinc_Code.Compiler.Models;

public class CompilerOptions
{
    public string OptimizationLevel { get; set; } = "O2";
    public string LanguageStandard { get; set; } = "c++17";
    public bool StaticLink { get; set; } = true;
    public bool EnableWarnings { get; set; } = true;
    public int TimeoutSeconds { get; set; } = 10;
    public Dictionary<string, string> AdditionalArguments { get; set; } = new();
}