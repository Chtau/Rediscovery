using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.TerminalPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TerminalFeaturePage : ContentPage
    {
        private IFeatureExchange featureExchange => DependencyService.Get<IFeatureExchange>() ?? new FeatureExchange();
        DesktopFeaturePageDetailViewModel viewModel;

        public TerminalFeaturePage(DesktopFeaturePageDetailViewModel model)
        {
            InitializeComponent();

            BindingContext = viewModel = model;
            featureExchange.Init(model.Connection);
            featureExchange.DesktopResponseReceived += FeatureExchange_DesktopResponseReceived;
            terminal.AddLines("Rediscovery Terminal Version " + model.ConnectionManifestFeature.FeatureVersion);
            terminal.SendCommand += Terminal_SendCommand;
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void Terminal_SendCommand(object sender, string e)
        {
            featureExchange.Send(viewModel.ConnectionManifestFeature, e);
        }

        private void FeatureExchange_DesktopResponseReceived(object sender, (Guid connectionId, Guid featureId, object data) e)
        {
            if (viewModel.ConnectionManifestFeature.ConnectionId == e.connectionId && viewModel.ConnectionManifestFeature.FeatureId == e.featureId)
            {
                terminal.AddLines(e.data?.ToString());
            }
        }
    }
}