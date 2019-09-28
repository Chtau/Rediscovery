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

        private IDataStoreGuid<Features.Authentication.Models.Connection> connectionStore => DependencyService.Get<IDataStoreGuid<Features.Authentication.Models.Connection>>() ?? new Features.Authentication.ConnectionStore();

        public ObservableCollection<Features.Authentication.Models.Connection> Items { get; set; }

        public MainPageViewModel()
        {
            
        }

        public void Load()
        {
            Task.Run(async () =>
            {
                Items = new ObservableCollection<Features.Authentication.Models.Connection>(await connectionStore.GetItemsAsync());
            });
        }
    }
}
