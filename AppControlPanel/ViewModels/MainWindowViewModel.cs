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

        public System.Collections.ObjectModel.ObservableCollection<SharedConfigurations.AppControlPanel.Models.AppViewModel> Apps { get; set; } = new System.Collections.ObjectModel.ObservableCollection<SharedConfigurations.AppControlPanel.Models.AppViewModel>();

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
            var appsSettings = Program.Configuration.GetSection(SharedConfigurations.AppControlPanel.Models.AppViewModel.SectionName).Get<SharedConfigurations.AppControlPanel.Models.AppViewModel[]>();
            if (appsSettings != null)
            {
                Apps = new System.Collections.ObjectModel.ObservableCollection<SharedConfigurations.AppControlPanel.Models.AppViewModel>(appsSettings);
            } else
            {
                Apps.Clear();
            }
        }

        public void StartItem(SharedConfigurations.AppControlPanel.Models.AppViewModel item)
        {
            _applicationStartService.Start(item);
        }
    }
}
