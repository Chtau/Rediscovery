using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PluginFeature
{
    public abstract class BaseDeviceFeature : BaseFeature<PluginFeatureData, PluginFeatureDefinition>, IDeviceFeatureImplementation
    {
        public virtual string OnGetUIZipPath(string zipFileName, string subDirectory)
        {
            string archivePath = Path.Combine(pluginDirectory, zipFileName);
            string uiDirectory = Path.Combine(pluginDirectory, subDirectory);
            if (System.IO.Directory.Exists(uiDirectory))
            {
                if (File.Exists(archivePath))
                    File.Delete(archivePath);
                ZipFile.CreateFromDirectory(uiDirectory, archivePath);
                return archivePath;
            }
            return null;
        }

        public string GetUIArchivePath()
        {
            return OnGetUIZipPath("ui.zip", "UI");
        }

        public virtual PluginFeatureSetting GetSettingsObject()
        {
            return null;
        }

        public virtual List<PluginFeatureProfil> GetProfiles()
        {
            return null;
        }

        public virtual void OpenSettingConfiguration()
        {
            
        }

        public virtual void OpenProfileConfiguration()
        {
            
        }
    }
}
