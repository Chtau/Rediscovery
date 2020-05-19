using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rediscovery.Services;
using Rediscovery.Views;
using Rediscovery.Features.Connection;

namespace Rediscovery
{
    public partial class App : Application
    {
        private IConnectService connect => DependencyService.Get<IConnectService>() ?? new ConnectService();

        public App()
        {
            InitializeComponent();

            DependencyService.Register<CommunicationClientConsumer.Hub>();

            MainPage = new MainPage();
        }

        protected async override void OnStart()
        {
            OnRestart();
        }

        protected async override void OnSleep()
        {
            
        }

        protected async override void OnResume()
        {
            OnRestart();
        }

        private void OnRestart()
        {
            connect.AutoConnect((result, state) =>
            {

            });
        }
    }
}
