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

        private IDataStoreGuid<Features.Connection.Models.ConnectionInfo> connectionStore => DependencyService.Get<IDataStoreGuid<Features.Connection.Models.ConnectionInfo>>() ?? new Features.Connection.ConnectionStore();

        public ObservableCollection<Features.Connection.Models.ConnectionInfo> Items { get; set; } = new ObservableCollection<Features.Connection.Models.ConnectionInfo>();

        public MainPageViewModel()
        {
            
        }

        public void Load()
        {
            Items.Clear();
            Task.Run(async () =>
            {
                var items = await connectionStore.GetItemsAsync();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            });
        }
    }
}
