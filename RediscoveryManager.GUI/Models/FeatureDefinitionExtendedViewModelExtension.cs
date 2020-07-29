using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.GUI.Models
{
    public class FeatureDefinitionExtendedViewModelExtension : SharedBase.Device.FeatureDefinitionExtended
    {
        private Action<FeatureDefinitionExtendedViewModelExtension> onOpenFolderCallback;
        private Action<FeatureDefinitionExtendedViewModelExtension> onOpenDesktopExecutableCallback;

        public FeatureDefinitionExtendedViewModelExtension(SharedBase.Device.FeatureDefinitionExtended feature, Action<FeatureDefinitionExtendedViewModelExtension> onOpenFolderCallback,
            Action<FeatureDefinitionExtendedViewModelExtension> onOpenDesktopExecutableCallback)
        {
            this.onOpenDesktopExecutableCallback = onOpenDesktopExecutableCallback;
            this.onOpenFolderCallback = onOpenFolderCallback;
            this.Author = feature.Author;
            this.ClientDescription = feature.ClientDescription;
            this.ControlIntegrationPoint = feature.ControlIntegrationPoint;
            this.DisplayName = feature.DisplayName;
            this.Documentation = feature.Documentation;
            this.FeatureIntegrationPoint = feature.FeatureIntegrationPoint;
            this.HasProfilConfiguration = feature.HasProfilConfiguration;
            this.HasSettingConfiguration = feature.HasSettingConfiguration;
            this.Id = feature.Id;
            this.IsClientImplementation = feature.IsClientImplementation;
            this.MinimalControlIntegrationPoint = feature.MinimalControlIntegrationPoint;
            this.MinimalFeatureIntegrationPoint = feature.MinimalFeatureIntegrationPoint;
            this.NativeResources = feature.NativeResources;
            this.PluginDirectory = feature.PluginDirectory;
            this.Version = feature.Version;
            this.Website = feature.Website;
            this.DesktopExecutable = feature.DesktopExecutable;
        }

        public void OpenFolder()
        {
            onOpenFolderCallback?.Invoke(this);
        }

        public void OpenDesktopExecutable()
        {
            onOpenDesktopExecutableCallback?.Invoke(this);
        }
    }
}
