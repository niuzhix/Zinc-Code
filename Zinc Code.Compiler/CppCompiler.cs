using System;
using System.Threading;
using System.Threading.Tasks;
using Zinc_Code.Compiler.Interfaces;
using Zinc_Code.Compiler.Models;
using Zinc_Code.Compiler.MinGW;

namespace Zinc_Code.Compiler;

public class CppCompiler : ICppCompiler
{
    private readonly ICppCompiler _innerCompiler;

    public bool IsAvailable => _innerCompiler.IsAvailable;

    public CppCompiler(string mingwPath)
    {
        if (string.IsNullOrWhiteSpace(mingwPath))
            throw new ArgumentException("MinGW路径不能为空", nameof(mingwPath));

        _innerCompiler = new MinGWCompiler(mingwPath);
    }

    public Task<CompilationResult> CompileAsync(
        CompilationRequest request,
        CancellationToken cancellationToken = default)
    {
        return _innerCompiler.CompileAsync(request, cancellationToken);
    }
}