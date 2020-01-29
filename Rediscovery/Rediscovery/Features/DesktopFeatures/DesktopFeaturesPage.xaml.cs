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

            //await Navigation.PushModalAsync(new NavigationPage(new FeaturePage.DesktopFeaturePageDetail(new FeaturePage.DesktopFeaturePageDetailViewModel(viewModel.Connection, item))));

            FeatureControl.SelectedItem = null;
        }
    }
}