using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ReactiveUI;
using System.Reactive;

namespace AppControlPanel.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Services.IApplicationStartService _applicationStartService;

        public string Greeting => "Welcome to Avalonia!";

        public System.Collections.ObjectModel.ObservableCollection<AppViewModel> Apps { get; set; } = new System.Collections.ObjectModel.ObservableCollection<AppViewModel>();

        public MainWindowViewModel()
        {
            _applicationStartService = (Services.IApplicationStartService)Program.ServiceProvider.GetService(typeof(Services.IApplicationStartService));

            SetAppsCollection();
            var token = Program.Configuration.GetReloadToken();
            token.RegisterChangeCallback(changed =>
            {
                SetAppsCollection();
            }, null);
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
            var result = _applicationStartService.Start(model.AppModel);
            model.AppLaunchState = result ? AppViewModel.LaunchState.Running : AppViewModel.LaunchState.Error;
        }
    }
}
