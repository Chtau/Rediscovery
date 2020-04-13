using System;
using System.IO;
using AppControlPanel.ViewModels;
using AppControlPanel.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppControlPanel
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfigurationRoot Configuration { get; private set; }

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
        .AddJsonFile(SharedConfigurations.AppControlPanel.ConfigFileNames.AppSettings, optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            //var appsSettings = Configuration.GetSection(SharedConfigurations.AppControlPanel.Models.AppViewModel.SectionName).Get<SharedConfigurations.AppControlPanel.Models.AppViewModel>();


            var service = new ServiceCollection();
            /*service.AddSingleton<IPCPipe.IPipeServer, IPCPipe.PipeServer>();
            service.AddSingleton<IPCPipe.IPipeClient, IPCPipe.PipeClient>();
            service.AddSingleton<IPCPipe.IPipeResourceProvider, IPCPipe.PipeResourceProvider>();
            service.AddSingleton<Connection.IIncomingConnectionPipe, Connection.IncomingConnectionPipe>();
            service.AddSingleton<Connection.IIncomingConnectionPipeLiveLogger, Connection.IncomingConnectionPipeLiveLogger>();
            service.AddSingleton<Connection.IIncomingConnectionService, Connection.IncomingConnectionService>();*/
            ServiceProvider = service.BuildServiceProvider();

            /*var incomingConnectionService = (Connection.IIncomingConnectionService)Program.ServiceProvider.GetService(typeof(Connection.IIncomingConnectionService));
            incomingConnectionService.Init();
            if (args.Any(x => x.StartsWith(SharedCommandArguments.Hub.Arguments.ServiceInfoStart, StringComparison.OrdinalIgnoreCase)))
                app.Run(new Info.ServiceInfo(serviceInfoSettings));
            else if (args.Any(x => x.StartsWith(SharedCommandArguments.Hub.Arguments.CodeArgStart, StringComparison.OrdinalIgnoreCase)))
                app.Run(new Connection.IncomingConnection(new Connection.Models.IncomingConnectionViewModel(args)));
            else*/
            app.Run(new MainWindow()
            {
                DataContext = new MainWindowViewModel()
            });
        }
    }
}
