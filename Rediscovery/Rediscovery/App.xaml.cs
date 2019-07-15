using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rediscovery.Services;
using Rediscovery.Views;

namespace Rediscovery
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<Logger>();
            DependencyService.Register<MockDataStore>();
            DependencyService.Register<DBStore>();
            DependencyService.Register<DesktopConfiguration.DesktopConfigurationStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
