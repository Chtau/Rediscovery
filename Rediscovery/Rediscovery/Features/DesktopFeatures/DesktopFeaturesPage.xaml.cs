using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesktopFeaturesPage : ContentPage
    {
        DesktopFeaturesViewModel viewModel;

        public DesktopFeaturesPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new DesktopFeaturesViewModel();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.Load();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            Features.Authentication.Models.ConnectionManifestFeature item = args.SelectedItem as Features.Authentication.Models.ConnectionManifestFeature;
            if (item == null)
                return;
            switch (item.ControlIntegration)
            {
                case SharedCoreModels.DeviceFeature.DeviceFeature.ControlIntegrationType.MediaPlayer:
                    await Navigation.PushModalAsync(new NavigationPage(new FeaturePage.MediaPlayer.MediaPlayerFeaturePage()));
                    break;
                case SharedCoreModels.DeviceFeature.DeviceFeature.ControlIntegrationType.Terminal:
                    await Navigation.PushModalAsync(new NavigationPage(new FeaturePage.TerminalPage.TerminalFeaturePage(new FeaturePage.TerminalPage.DesktopFeaturePageDetailViewModel(item))));
                    break;
                default:
                    break;
            }

            FeatureControl.SelectedItem = null;
        }
    }
}