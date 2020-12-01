using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Rediscovery.Shared.Logging.ILogger _logger;
        private readonly Shared.ISharedEvents _sharedEvents;

        public MainWindowViewModel()
        {
            _logger = Locator.Current.GetService<Rediscovery.Shared.Logging.ILogger>();
            _sharedEvents = Locator.Current.GetService<Shared.ISharedEvents>();
            _sharedEvents.LoadingState += _sharedEvents_LoadingState;
            IsLoading = _sharedEvents.HasLoadingState();
        }

        private void _sharedEvents_LoadingState(object sender, bool e)
        {
            try
            {
                IsLoading = e;
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public string Greeting => "Welcome to Avalonia!";

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                this.RaiseAndSetIfChanged(ref isLoading, value);
            }
        }
    }
}
