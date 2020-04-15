using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppControlPanel.ViewModels
{
    public class AppViewModel : ViewModelBase
    {
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

        }

        public AppViewModel(SharedConfigurations.AppControlPanel.Models.AppModel appModel) : this()
        {
            AppModel = appModel;
        }
    }
}
