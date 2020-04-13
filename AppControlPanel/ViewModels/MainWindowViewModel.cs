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
            try
            {
                if (Apps.Any(x => x.AppModel.AutoStartWithPanel))
                {
                    foreach (var item in Apps.Where(x => x.AppModel.AutoStartWithPanel))
                    {
                        item.AppLaunchState = _applicationStartService.Start(item.AppModel);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Auto start Apps with Panel Exception:" + ex.ToString());
            }
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
                            if (item.AppModel.AutoStartWhenNotRunning && item.AppLaunchState == AppViewModel.LaunchState.NotRunning)
                            {
                                item.AppLaunchState = _applicationStartService.Start(item.AppModel);
                            }
                        }
                        await Task.Delay(1000);
                    } catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print("Watch loop Exception:" + ex.ToString());
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
        }
    }
}
