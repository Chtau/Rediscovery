using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace AppControlPanel.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Services.IApplicationStartService _applicationStartService;
        private readonly Services.IApplicationWatchService _applicationWatchService;

        public System.Collections.ObjectModel.ObservableCollection<AppViewModel> Apps { get; set; } = new System.Collections.ObjectModel.ObservableCollection<AppViewModel>();

        public MainWindowViewModel()
        {
            _applicationStartService = (Services.IApplicationStartService)Program.ServiceProvider.GetService(typeof(Services.IApplicationStartService));
            _applicationWatchService = (Services.IApplicationWatchService)Program.ServiceProvider.GetService(typeof(Services.IApplicationWatchService));

            SetAppsCollection();
            var token = Program.Configuration.GetReloadToken();
            token.RegisterChangeCallback(changed =>
            {
                SetAppsCollection();
            }, null);
            Task.Run(async () =>
            {
                do
                {
                    try
                    {
                        foreach (var item in Apps)
                        {

                            _applicationWatchService.Watch(item.AppModel, (state, prcId) =>
                            {
                                item.AppLaunchState = state;
                                item.ProcessId = prcId;
                            });
                        }
                        await Task.Delay(1000);
                    } catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print("Watch loop:" + ex.ToString());
                    }
                } while (true);
            });
        }

        private void SetAppsCollection()
        {
            var appsSettings = Program.Configuration.GetSection(SharedConfigurations.AppControlPanel.Models.AppModel.SectionName).Get<SharedConfigurations.AppControlPanel.Models.AppModel[]>();
            if (appsSettings != null)
            {
                Apps.Clear();
                foreach (var item in appsSettings)
                {
                    Apps.Add(new AppViewModel(item));
                }
            } else
            {
                Apps.Clear();
            }
        }

        public void StartItem(AppViewModel model)
        {
            model.AppLaunchState = _applicationStartService.Start(model.AppModel);
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                _applicationWatchService.Watch(model.AppModel, (state, prcId) =>
                {
                    model.AppLaunchState = state;
                    model.ProcessId = prcId;
                });
            });
        }
    }
}
