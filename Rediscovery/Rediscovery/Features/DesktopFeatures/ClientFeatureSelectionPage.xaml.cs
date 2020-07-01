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
    public partial class ClientFeatureSelectionPage : ContentPage
    {
        ClientFeatureSelectionViewModel viewModel;

        public ClientFeatureSelectionPage(ClientFeatureSelectionViewModel model)
        {
            InitializeComponent();

            BindingContext = viewModel = model;
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            Features.Connection.Models.ConnectionManifestFeature item = args.SelectedItem as Features.Connection.Models.ConnectionManifestFeature;
            if (item != null)
            {
                viewModel.SetSelectedFeaturer(item);
                FeatureControl.SelectedItem = null;
                await Navigation.PopModalAsync();
            }
        }
    }
}