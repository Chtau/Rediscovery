using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;

namespace Rediscovery.Features.DesktopConfiguration
{
    public class DesktopConfigurationViewModel : BaseViewModel
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        public IDataStoreGuid<DesktopConfigurationModel> Store => DependencyService.Get<IDataStoreGuid<DesktopConfigurationModel>>() ?? new DesktopConfigurationStore();

        public ObservableCollection<DesktopConfigurationModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }


        public DesktopConfigurationViewModel()
        {
            Title = "Desktop";
            Items = new ObservableCollection<DesktopConfigurationModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadDeviceItemsCommand());
            MessagingCenter.Subscribe<DesktopConfigurationEditViewModel, DesktopConfigurationModel>(this, "refresh_desktop_configuration", async (obj, args) =>
            {
                await ExecuteLoadDeviceItemsCommand();
            });
        }

        async Task ExecuteLoadDeviceItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await Store.GetItemsAsync(true);
                if (items != null && items.Count() > 0)
                {
                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
