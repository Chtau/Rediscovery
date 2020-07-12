using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using RediscoveryManager.GUI.ViewModels;
using RediscoveryManager.GUI.Views;
using RediscoveryManager.Service;
using Splat;
using System;

namespace RediscoveryManager.GUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            RegisterDependencies();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void RegisterDependencies()
        {
            Locator.CurrentMutable.RegisterConstant(SharedBase.Logging.DiagnosticsLoggerProvider.Instance, typeof(SharedBase.Logging.ILogger));
            Locator.CurrentMutable.RegisterConstant(new Manager(SharedBase.Logging.DiagnosticsLoggerProvider.Instance), typeof(IManager));
        }
    }
}
