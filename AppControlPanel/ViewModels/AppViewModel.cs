using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.ControlPanel.ViewModels
{
    public class AppViewModel : ViewModelBase
    {
        private readonly Services.IApplicationStartService _applicationStartService;
        private readonly Services.IApplicationWatchService _applicationWatchService;

        public enum LaunchState
        {
            None = 0,
            Running = 1,
            Error = 2,
            NotFound = 3,
            Starting = 4,
            ErrorStarting = 5,
            NotRunning = 6,
        }

        public SharedConfigurations.AppControlPanel.Models.AppModel AppModel { get; set; }

        private LaunchState appLaunchState = LaunchState.None;
        public LaunchState AppLaunchState
        {
            get => appLaunchState;
            set
            {
                this.RaiseAndSetIfChanged(ref appLaunchState, value);
                IsRunning = appLaunchState == LaunchState.Running;
            }
        }

        private int? processId = null;
        public int? ProcessId
        {
            get => processId;
            set => this.RaiseAndSetIfChanged(ref processId, value);
        }

        private bool isRunning = false;
        public bool IsRunning
        {
            get => isRunning;
            set => this.RaiseAndSetIfChanged(ref isRunning, value);
        }

        public List<int> AdditionalProcesses { get; set; } = new List<int>();

        public AppViewModel()
        {
            _applicationStartService = (Services.IApplicationStartService)Program.ServiceProvider.GetService(typeof(Services.IApplicationStartService));
            _applicationWatchService = (Services.IApplicationWatchService)Program.ServiceProvider.GetService(typeof(Services.IApplicationWatchService));

        }

        public AppViewModel(SharedConfigurations.AppControlPanel.Models.AppModel appModel) : this()
        {
            AppModel = appModel;
        }


        public void StartItem(AppViewModel model)
        {
            model.AppLaunchState = _applicationStartService.Start(model.AppModel,
                proc =>
                {
                    model.AdditionalProcesses.Add(proc);
                });
        }

        public void StopItem(AppViewModel model)
        {
            _applicationWatchService.Watch(model, (state, prcId) =>
            {
                model.AppLaunchState = state;
                model.ProcessId = prcId;
                if (model.AppLaunchState == AppViewModel.LaunchState.Running)
                {
                    try
                    {
                        var prc = System.Diagnostics.Process.GetProcessById(model.ProcessId.Value);
                        if (prc != null)
                            prc.Kill(true);
                        if (model.AdditionalProcesses.Count > 0)
                        {
                            for (int i = 0; i < model.AdditionalProcesses.Count; i++)
                            {
                                try
                                {
                                    if (System.Diagnostics.Process.GetProcesses().Any(x => x.Id == model.AdditionalProcesses[i]))
                                    {
                                        var prcTmp = System.Diagnostics.Process.GetProcessById(model.AdditionalProcesses[i]);
                                        if (prcTmp != null)
                                            prcTmp.Kill(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.Print("Stop additional Process Exception:" + ex.ToString());
                                }
                            }
                            model.AdditionalProcesses.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print("Stop Process Exception:" + ex.ToString());
                    }
                }
            });
        }
    }
}
