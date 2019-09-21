using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rediscovery.DesktopConfiguration
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

            MessagingCenter.Subscribe<DesktopConfigurationEditViewModel>(this, "refresh_desktop_configuration", async (obj) =>
            {
                await ExecuteLoadDeviceItemsCommand();
            });

            /*MessagingCenter.Subscribe<DesktopConfigurationEditPage, DesktopConfigurationModel>(this, "save_desktop_configuration", async (obj, item) =>
            {
                var _item = item as DesktopConfigurationModel;
                await Store.AddItemAsync(_item);
                await ExecuteLoadDeviceItemsCommand();
            });

            MessagingCenter.Subscribe<DesktopConfigurationEditPage, DesktopConfigurationModel>(this, "remove_desktop_configuration", async (obj, item) =>
            {
                DesktopConfigurationModel _item = Items.FirstOrDefault(x => x.Id == item.Id);
                if (_item != null)
                {
                    await Store.DeleteItemAsync(_item.Id);
                    await ExecuteLoadDeviceItemsCommand();
                }
            });*/

            /*MessagingCenter.Subscribe<Connections>(this, GlobalVariableNames.MessagingAfterTryConnect, async (obj) =>
            {
                await ExecuteLoadDeviceItemsCommand();
            });*/
        }

        async Task ExecuteLoadDeviceItemsCommand()
        {
            /*if (IsBusy)
                return;*/

            //IsBusy = true;

            try
            {
                Items.Clear();
                var items = await Store.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                //IsBusy = false;
            }
        }

    }
}
