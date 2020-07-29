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
                OpenDirectory(item.PluginDirectory);
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
                OpenDirectory(item.PluginDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private static void OpenDirectory(string directory, string filename = null, string extension = null)
        {
            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (!extension.StartsWith("."))
                    extension = "." + extension;
            }
            string filePath;
            if (!string.IsNullOrWhiteSpace(filename))
            {
                if (!string.IsNullOrWhiteSpace(extension))
                    filePath = System.IO.Path.Combine(directory, filename + extension);
                else
                    filePath = System.IO.Path.Combine(directory, filename);
            }
            else
            {
                filePath = directory;
            }
            string argument = "/select, \"" + filePath + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}
