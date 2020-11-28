using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace Rediscovery.Client.App.ControlPanel.ViewModels
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
                if (Apps.Any(x => x.AppModel.AutoStartWithPanel.HasValue && x.AppModel.AutoStartWithPanel.Value))
                {
                    foreach (var item in Apps.Where(x => x.AppModel.AutoStartWithPanel.HasValue && x.AppModel.AutoStartWithPanel.Value))
                    {
                        item.AppLaunchState = _applicationStartService.Start(item.AppModel, proc =>
                        {
                            item.AdditionalProcesses.Add(proc);
                        });
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
                            _applicationWatchService.Watch(item, (state, prcId) =>
                            {
                                item.AppLaunchState = state;
                                item.ProcessId = prcId;
                            });
                            if (item.AppModel.AutoStartWhenNotRunning.HasValue && item.AppModel.AutoStartWhenNotRunning.Value && item.AppLaunchState == AppViewModel.LaunchState.NotRunning)
                            {
                                item.AppLaunchState = _applicationStartService.Start(item.AppModel, proc =>
                                {
                                    item.AdditionalProcesses.Add(proc);
                                });
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
            var appsSettings = Program.Configuration.GetSection(Shared.Configurations.ControlPanel.Models.AppModel.SectionName).Get<Shared.Configurations.ControlPanel.Models.AppModel[]>();
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

        public void OpenSettings()
        {
            try
            {
                string path = System.IO.Path.Combine(Feature.Shared.Functions.File.GetApplicationFolder(), Shared.Configurations.ControlPanel.ConfigFileNames.AppSettings);
                Feature.Shared.Functions.File.OpenWithDefaultProgram(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Open Settings Exception:" + ex.ToString());
            }
        }
    }
}
