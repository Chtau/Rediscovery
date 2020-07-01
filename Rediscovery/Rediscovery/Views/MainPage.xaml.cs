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

            BindingContext = viewModel = new MainPageViewModel();
            viewModel.Load();
        }

        private async void ClientFeatureService_OpenFeatureSelectDialog(object sender, System.Collections.Generic.IEnumerable<Features.Connection.Models.ConnectionManifestFeature> e)
        {
            var model = new ClientFeatureSelectionViewModel(e, (feature) =>
            {
                if (feature != null)
                {
                    _logger.LogDebug($"[OpenFeatureSelectDialog] Feature (FeatureId:{feature.FeatureId}) selected");
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