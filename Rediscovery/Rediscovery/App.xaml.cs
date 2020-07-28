using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rediscovery.Services;
using Rediscovery.Views;
using Rediscovery.Features.Connection;
using SharedBase.Core;

namespace Rediscovery
{
    public partial class App : Application
    {
        public static SharedBase.Core.Version ClientVersion = new SharedBase.Core.Version() { Label = null, Major = 0, Minor = 0, Patch = 0 };
        private IConnectService connect => DependencyService.Get<IConnectService>() ?? new ConnectService();
        private Features.DesktopFeatures.IClientFeatureService clientFeatureService => DependencyService.Get<Features.DesktopFeatures.IClientFeatureService>() ?? new Features.DesktopFeatures.ClientFeatureService();

        public App()
        {
            InitializeComponent();

            DependencyService.Register<IConsumer, Consumer>();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            OnRestart();
        }

        protected override void OnResume()
        {
            OnRestart();
        }

        private void OnRestart()
        {
            connect.AutoConnect((result, state) => System.Diagnostics.Debug.Print($"AutoConnect Result:{result} State:{state}"));
        }

        public void OpenWithIntent(PluginFeature.Models.ClientResources.OpenWithIntent intentReceivedModel)
        {
            string title;
            if (!string.IsNullOrWhiteSpace(intentReceivedModel?.Title))
            {
                title = "File: " + intentReceivedModel.Title;
            } else if (!string.IsNullOrWhiteSpace(intentReceivedModel?.Uri))
            {
                title = "Url: " + intentReceivedModel.Uri;
            } else
            {
                title = "Share data";
            }
            clientFeatureService.Invoke(title, intentReceivedModel, SharedBase.Enums.ClientNativeResources.OpenWithIntent);
        }
    }
}
