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
            ErrorStarting = 5
        }

        public SharedConfigurations.AppControlPanel.Models.AppModel AppModel { get; set; }

        private LaunchState appLaunchState = LaunchState.None;
        public LaunchState AppLaunchState
        {
            get => appLaunchState;
            set => this.RaiseAndSetIfChanged(ref appLaunchState, value);
        }

        public AppViewModel()
        {

        }

        public AppViewModel(SharedConfigurations.AppControlPanel.Models.AppModel appModel) : this()
        {
            AppModel = appModel;
        }
    }
}
