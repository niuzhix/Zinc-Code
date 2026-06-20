using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HotAvalonia;
using System.Linq;
using Zinc_Code.ViewModels;
using Zinc_Code.Views;

namespace Zinc_Code
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            this.UseHotReload();
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();

                var storageProvider = mainWindow.StorageProvider;

                mainWindow.DataContext = new MainWindowViewModel(storageProvider);

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}