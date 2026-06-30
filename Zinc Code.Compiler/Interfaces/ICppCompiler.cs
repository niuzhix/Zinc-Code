using System.Threading;
using System.Threading.Tasks;
using Zinc_Code.Compiler.Models;

namespace Zinc_Code.Compiler.Interfaces;

public interface ICppCompiler
{
    /// <summary>
    /// 检查编译器是否可用
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// 编译C++源代码
    /// </summary>
    Task<CompilationResult> CompileAsync(
        CompilationRequest request,
        CancellationToken cancellationToken = default);
}