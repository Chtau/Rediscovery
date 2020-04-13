using System;
using System.Collections.Generic;
using System.Text;

namespace AppControlPanel.ViewModels
{
    public class AppViewModel : ViewModelBase
    {
        public SharedConfigurations.AppControlPanel.Models.AppModel AppModel { get; set; }

        public AppViewModel()
        {

        }

        public AppViewModel(SharedConfigurations.AppControlPanel.Models.AppModel appModel) : this()
        {
            AppModel = appModel;
        }
    }
}
