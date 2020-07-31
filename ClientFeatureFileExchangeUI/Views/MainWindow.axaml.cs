using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClientFeatureFileExchangeUI.ViewModels;
using System;

namespace ClientFeatureFileExchangeUI.Views
{
    public class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;
        private IPCPipe.IPipeExchange pipeExchange;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = viewModel = new MainWindowViewModel((Window)this.VisualRoot);

            pipeExchange = new IPCPipe.PipeExchange();
            pipeExchange.Init("7C7BE7CA-DE13-4975-A099-C64FA1581E4A", "B", "A");
            pipeExchange.DataReceived += (obj, args) =>
            {
                System.Diagnostics.Debug.Print($"IPCServer on {nameof(MainWindow)} Hub received data:{args}");
                pipeExchange.Send($"Send from Client {DateTime.Now}");
            };
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
