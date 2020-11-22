using ReactiveUI;
using Rediscovery.Client.App.Manager.GUI.Models;
using SharedFeatureFunctions;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.ViewModels
{
    public class FeaturesViewModel : ViewModelBase
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

        private ObservableCollection<FeatureDefinitionExtendedViewModelExtension> items;
        public ObservableCollection<FeatureDefinitionExtendedViewModelExtension> Items
        {
            get { return items; }
            set
            {
                this.RaiseAndSetIfChanged(ref items, value);
            }
        }

        public FeaturesViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();
            _sharedEvents = Locator.Current.GetService<Shared.ISharedEvents>();

            _manager.FeaturesCollectionChanged += (obj, args) =>
            {
                var newCollection = new List<FeatureDefinitionExtendedViewModelExtension>();
                foreach (var item in _manager.Features)
                {
                    newCollection.Add(new FeatureDefinitionExtendedViewModelExtension(item, OnOpenFolderCallback, OnOpenDesktopExecutableCallback));
                }
                Items = new ObservableCollection<FeatureDefinitionExtendedViewModelExtension>(newCollection);
            };
        }

        private void OnOpenFolderCallback(FeatureDefinitionExtendedViewModelExtension item)
        {
            try
            {
                File.OpenDirectory(item.PluginDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void OnOpenDesktopExecutableCallback(FeatureDefinitionExtendedViewModelExtension item)
        {
            try
            {
                File.OpenDirectory(item.PluginDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
