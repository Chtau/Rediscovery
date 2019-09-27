using System;
using System.Linq;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopHub
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }


        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            var service = new ServiceCollection();
            service.AddSingleton<IPCPipe.IPipeServer, IPCPipe.PipeServer>();
            service.AddSingleton<IPCPipe.IPipeClient, IPCPipe.PipeClient>();
            service.AddSingleton<IPCPipe.IPipeResourceProvider, IPCPipe.PipeResourceProvider>();
            service.AddSingleton<Connection.IIncomingConnectionPipe, Connection.IncomingConnectionPipe>();
            service.AddSingleton<Connection.IIncomingConnectionService, Connection.IncomingConnectionService>();
            ServiceProvider = service.BuildServiceProvider();

            var incomingConnectionService = (Connection.IIncomingConnectionService)Program.ServiceProvider.GetService(typeof(Connection.IIncomingConnectionService));
            incomingConnectionService.Init();

            if (args.Any(x => x.StartsWith(Connection.Models.IncomingConnectionViewModel.CodeArgStart, StringComparison.OrdinalIgnoreCase)))
                app.Run(new Connection.IncomingConnection(new Connection.Models.IncomingConnectionViewModel(args)));
            else
                app.Run(new MainWindow());
        }

    }
}
