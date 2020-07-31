using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClientFeatureFileExchangeUI.ViewModels;

namespace ClientFeatureFileExchangeUI.Views
{
    public class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;
        private IPCPipe.IPipeClient pipeClient;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = viewModel = new MainWindowViewModel((Window)this.VisualRoot);

            pipeClient = new IPCPipe.PipeClient();
            //pipeClient.DataReceived += PipeClient_DataReceived;
            pipeClient.TryConnect("7C7BE7CA-DE13-4975-A099-C64FA1581E4A");
            pipeClient.Send("7C7BE7CA-DE13-4975-A099-C64FA1581E4A", "test");
            var ipcServer = new IPCPipe.PipeServer();
            ipcServer.Listen("7C7BE7CA-DE13-4975-A099-C64FA1581E4A_1", (data) =>
            {
                System.Diagnostics.Debug.Print($"IPCServer on {nameof(MainWindow)} Hub received data:{data}");
                pipeClient.Send("7C7BE7CA-DE13-4975-A099-C64FA1581E4A", "test");
            });
        }

        private void PipeClient_DataReceived(object sender, string e)
        {
            System.Diagnostics.Debug.Print($"IPCClient on {nameof(MainWindow)} Hub received data:{e}");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
