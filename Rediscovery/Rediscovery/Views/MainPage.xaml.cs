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

        private Features.Connection.IConnect auth => DependencyService.Get<Features.Connection.IConnect>() ?? new Features.Connection.Connect();

        public MainPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new MainPageViewModel();
            viewModel.Load();
            auth.HelloReceived += Auth_HelloReceived;
        }

        private void Auth_HelloReceived(object sender, Features.Connection.Models.ConnectionInfo e)
        {
            if (e.ConnectionState == SharedCoreModels.Enums.ConnectionState.WaitForApprovel)
            {
                Navigation.PushModalAsync(new Features.Authentication.AuthenticationKey(e.Id));
            }
        }
    }
}