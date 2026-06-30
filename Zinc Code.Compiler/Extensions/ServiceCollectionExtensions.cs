using Microsoft.Extensions.DependencyInjection;
using Zinc_Code.Compiler.Interfaces;

namespace Zinc_Code.Compiler.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCppCompiler(
        this IServiceCollection services,
        string mingwPath)
    {
        services.AddSingleton<ICppCompiler>(provider =>
            new CppCompiler(mingwPath));

        return services;
    }
}