using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rediscovery.Views
{
    public class MainPageViewModel : BaseViewModel
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();

        private IDataStoreGuid<Features.DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<Features.DesktopConfiguration.DesktopConfigurationModel>>() ?? new Features.DesktopConfiguration.DesktopConfigurationStore();

        public ObservableCollection<Features.DesktopConfiguration.DesktopConfigurationModel> Items { get; set; } = new ObservableCollection<Features.DesktopConfiguration.DesktopConfigurationModel>();

        public MainPageViewModel()
        {
            
        }

        public void Load()
        {
            Items.Clear();
            Task.Run(async () =>
            {
                var items = await desktopStore.GetItemsAsync();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            });
        }
    }
}
