using Rediscovery.Services;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : TabbedPage
    {
        private MainPageViewModel viewModel;

        private Features.Authentication.IConnect auth => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();
        private Services.IDiscoveryService discoveryService => DependencyService.Get<Services.IDiscoveryService>() ?? new Services.DiscoveryService();

        public MainPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new MainPageViewModel();
            viewModel.Load();
            auth.HelloReceived += Auth_HelloReceived;
            OnSendDiscovery();
        }

        private void Auth_HelloReceived(object sender, Features.Authentication.Models.Connection e)
        {
            if (e.ConnectionState == SharedCoreModels.Enums.ConnectionState.WaitForApprovel)
            {
                Navigation.PushModalAsync(new Features.Authentication.AuthenticationKey(e.Id));
            }
        }

        private void OnSendDiscovery()
        {
            discoveryService.Boardcast((answer) =>
            {
                Console.WriteLine("Anwser Recived from IPAddress:{0}", answer);
            });
            /*Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(1000);
                    var Client = new UdpClient();
                    var RequestData = Encoding.ASCII.GetBytes("SomeRequestData");
                    var ServerEp = new IPEndPoint(IPAddress.Any, 0);

                    Client.EnableBroadcast = true;
                    Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

                    //var ServerResponseData = Client.Receive(ref ServerEp);
                    //var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                    //Console.WriteLine("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString());
                    
                    Client.Close();
                } while (true);
            });*/
        }
    }
}