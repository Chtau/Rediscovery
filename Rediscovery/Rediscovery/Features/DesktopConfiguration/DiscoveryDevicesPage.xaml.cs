using Rediscovery.Services;
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
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();

        DiscoveryDevicesViewModel viewModel;

        public DiscoveryDevicesPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new DiscoveryDevicesViewModel();
        }

        private async void DevicesFoundControl_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SharedCoreModels.DiscoveryServiceInfo item = e.SelectedItem as SharedCoreModels.DiscoveryServiceInfo;
            if (item != null)
            {
                string answer = await DisplayPromptAsync("Save Desktop", "Save new Desktop with the following Name", "Yes", "Cancel", placeholder:"Desktop Name", initialValue:item.Name);
                if (!string.IsNullOrWhiteSpace(answer))
                {
                    viewModel.StopDiscoveryCommand.Execute(null);
                    var newDevice = new DesktopConfigurationModel
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = answer,
                        LastKnownAddress = item.IPAddress + ":" + item.Port,
                        AutoConnect = false,
                        ConnectionState = SharedCoreModels.Enums.ConnectionState.None,
                        LastConnection = null
                    };
                    var result = await desktopStore.AddItemAsync(newDevice);
                    if (result.Item1)
                    {
                        MessagingCenter.Send(this, "refresh_desktop_configuration", newDevice);
                    }
                    await Navigation.PopModalAsync();
                }
                DevicesFoundControl.SelectedItem = null;
            }
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            viewModel.StopDiscoveryCommand.Execute(null);
            await Navigation.PopModalAsync();
        }
    }
}