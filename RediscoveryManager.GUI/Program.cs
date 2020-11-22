using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rediscovery.Client.App.Manager.GUI.ViewModels;
using Rediscovery.Client.App.Manager.GUI.Views;
using Splat;

namespace Rediscovery.Client.App.Manager.GUI
{
    public static class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }
        public static Window MainWindow { get; private set; }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .Start(AppMain, args);
        //.StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        private static void AppMain(Application app, string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(SharedConfigurations.RediscoveryManager.GUI.ConfigFileNames.AppSettings, optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            RegisterDependencies();
            MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel()
            };
            RegisterDependencies(MainWindow);
            app.Run(MainWindow);
        }

        private static void RegisterDependencies()
        {
            var jsonSetting = Program.Configuration.GetSection(SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration.SectionName).Value;
            var connectSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration>(jsonSetting);// Program.Configuration.GetSection(SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration.SectionName).Get<SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration>();
            Locator.CurrentMutable.RegisterConstant(connectSettings, typeof(SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration));
            var manager = new Manager();
            Locator.CurrentMutable.RegisterConstant(manager, typeof(IManager));
            Locator.CurrentMutable.RegisterConstant(SharedBase.Logging.DiagnosticsLoggerProvider.Instance, typeof(SharedBase.Logging.ILogger));
            Locator.CurrentMutable.RegisterConstant(new Shared.SharedEvents(), typeof(Shared.ISharedEvents));
        }

        private static void RegisterDependencies(Window window)
        {
            Locator.CurrentMutable.RegisterConstant(new Notifications.NotificationService(window), typeof(Notifications.INotificationService));
        }
    }
}
