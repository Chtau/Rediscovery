using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopConfiguration
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscoveryDevicesPage : ContentPage
    {
        DiscoveryDevicesViewModel viewModel;

        public DiscoveryDevicesPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new DiscoveryDevicesViewModel();
        }

        private void DevicesFoundControl_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SharedCoreModels.DiscoveryServiceInfo item = e.SelectedItem as SharedCoreModels.DiscoveryServiceInfo;
            if (item != null)
            {
                //await Navigation.PushModalAsync(new NavigationPage(new FeaturePage.FeatureView.FeatureView(item.ConfigurationId, item)));
                DevicesFoundControl.SelectedItem = null;
            }
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}