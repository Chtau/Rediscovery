using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Desktops.DesktopFeaturePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesktopFeaturePageDetail : ContentPage
    {
        private IFeatureExchange featureExchange => DependencyService.Get<IFeatureExchange>() ?? new FeatureExchange();

        DesktopFeaturePageDetailViewModel viewModel;

        public DesktopFeaturePageDetail(DesktopFeaturePageDetailViewModel model)
        {
            InitializeComponent();

            BindingContext = viewModel = model;

            featureExchange.DesktopResponseReceived += FeatureExchange_DesktopResponseReceived;
            terminal.AddLines("Rediscovery Terminal Version " + model.ConnectionManifestFeature.FeatureVersion);
            terminal.SendCommand += Terminal_SendCommand;
        }

        private void FeatureExchange_DesktopResponseReceived(object sender, (Guid connectionId, Guid featureId, object data) e)
        {
            if (viewModel.ConnectionManifestFeature.ConnectionId == e.connectionId && viewModel.ConnectionManifestFeature.FeatureId == e.featureId)
            {
                terminal.AddLines(e.data?.ToString());
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void Terminal_SendCommand(object sender, string e)
        {
            featureExchange.Send(viewModel.Connection, viewModel.ConnectionManifestFeature, e);
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}