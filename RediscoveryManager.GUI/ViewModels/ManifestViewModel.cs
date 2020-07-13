using ReactiveUI;
using RediscoveryManager.GUI.Models;
using RediscoveryManager.Service;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RediscoveryManager.GUI.ViewModels
{
    public class ManifestViewModel : ViewModelBase
    {
        private readonly IManager _manager;
        private readonly SharedBase.Logging.ILogger _logger;
        private readonly Shared.ISharedEvents _sharedEvents;
        private Notifications.INotificationService _notification;

        public Notifications.INotificationService Notification
        {
            get
            {
                if (_notification == null)
                    _notification = Locator.Current.GetService<Notifications.INotificationService>();
                return _notification;
            }
        }

        private ManifestViewModelExtension item;
        public ManifestViewModelExtension Item
        {
            get { return item; }
            set
            {
                this.RaiseAndSetIfChanged(ref item, value);
            }
        }

        public ManifestViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();
            _sharedEvents = Locator.Current.GetService<Shared.ISharedEvents>();

            _manager.ManifestChanged += (obj, args) =>
            {
                Item = new ManifestViewModelExtension(_manager.Manifest);
            };
            Item = new ManifestViewModelExtension(_manager.Manifest);
        }
    }
}
