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

        private IConnect connect => DependencyService.Get<IConnect>() ?? new Connect();

        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected async override void OnStart()
        {
            // Handle when your app starts
            await connect.AutoConnect();
        }

        protected async override void OnSleep()
        {
            // Handle when your app sleeps
            await connect.CloseConnections();
        }

        protected async override void OnResume()
        {
            // Handle when your app resumes
            await connect.AutoConnect();
        }
    }
}
