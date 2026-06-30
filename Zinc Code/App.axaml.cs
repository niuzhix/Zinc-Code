using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HotAvalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Zinc_Code.Compiler;
using Zinc_Code.Compiler.Interfaces;
using Zinc_Code.ViewModels;
using Zinc_Code.Views;

namespace Zinc_Code;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        this.UseHotReload();
        AvaloniaXamlLoader.Load(this);
        BuildServiceProvider();
    }

    private void BuildServiceProvider()
    {
        var services = new ServiceCollection();

        // 注册编译器
        string mingwPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Tools", "MinGW");

        services.AddSingleton<ICppCompiler>(provider => new CppCompiler(mingwPath));

        // 注册 MainWindowViewModel 工厂
        services.AddTransient<Func<Avalonia.Platform.Storage.IStorageProvider, MainWindowViewModel>>(
            provider => storageProvider =>
                new MainWindowViewModel(storageProvider, provider.GetRequiredService<ICppCompiler>()));

        _serviceProvider = services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            var storageProvider = mainWindow.StorageProvider;

            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider has not been built.");

            var viewModelFactory = _serviceProvider.GetRequiredService<Func<Avalonia.Platform.Storage.IStorageProvider, MainWindowViewModel>>();
            mainWindow.DataContext = viewModelFactory(storageProvider);

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}