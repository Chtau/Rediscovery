using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rediscovery.Services;
using Rediscovery.Views;

namespace Rediscovery
{
    public partial class App : Application
    {

        private Features.Authentication.IConnect connect => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();

        public App()
        {
            InitializeComponent();

            DependencyService.Register<Logger>();
            DependencyService.Register<DBStore>();
            DependencyService.Register<Features.DesktopConfiguration.DesktopConfigurationStore>();
            DependencyService.Register<Features.Authentication.Connect>();
            DependencyService.Register<Features.Authentication.ConnectionStore>();
            DependencyService.Register<Features.Authentication.ConnectionManifestFeatureStore>();
            DependencyService.Register<Desktops.FeatureExchange>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            connect.AutoConnect();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            connect.CloseConnections();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            connect.AutoConnect();
        }
    }
}
