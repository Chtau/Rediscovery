using Rediscovery.Features.DesktopFeatures;
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
        private SharedBase.Logging.ILogger _logger => DependencyService.Get<SharedBase.Logging.ILogger>() ?? new Logger();
        private Features.DesktopFeatures.IClientFeatureService clientFeatureService => DependencyService.Get<Features.DesktopFeatures.IClientFeatureService>() ?? new Features.DesktopFeatures.ClientFeatureService();
        private MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();

            clientFeatureService.OpenFeatureSelectDialog += ClientFeatureService_OpenFeatureSelectDialog;
            clientFeatureService.RemoteFeatureRequest += ClientFeatureService_RemoteFeatureRequest;
            BindingContext = viewModel = new MainPageViewModel();
            viewModel.Load();
        }

        private void ClientFeatureService_RemoteFeatureRequest(object sender, ClientFeatureService.RemoteFeatureRequestState e)
        {
            string message = "";
            switch (e)
            {
                case ClientFeatureService.RemoteFeatureRequestState.NoFeatures:
                    message = "No remote features found. Check if you are connected to a device.";
                    break;
                case ClientFeatureService.RemoteFeatureRequestState.MissingSupport:
                    message = "The devices you are currently connected don't support the request.";
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrWhiteSpace(message))
            {
                DisplayAlert("Client feature", message, "Cancel");
            }
        }

        private async void ClientFeatureService_OpenFeatureSelectDialog(object sender, System.Collections.Generic.IEnumerable<Features.Connection.Models.ConnectionManifestFeature> e)
        {
            var model = new ClientFeatureSelectionViewModel(e, (feature) =>
            {
                if (feature != null)
                {
                    _logger.LogDebug($"[OpenFeatureSelectDialog] Feature (FeatureId:{feature.FeatureId}) selected");
                    clientFeatureService.SelectFeatureSelected(feature);
                }
                else
                {
                    _logger.LogWarning("[OpenFeatureSelectDialog] user selected no feature to use");
                }
            });
            await Navigation.PushModalAsync(new NavigationPage(new ClientFeatureSelectionPage(model)));
        }
    }
}