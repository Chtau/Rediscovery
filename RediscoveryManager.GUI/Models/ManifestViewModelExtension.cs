using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using Rediscovery.Client.App.Manager.GUI.ViewModels;

namespace Rediscovery.Client.App.Manager.GUI.Models
{
    public class ManifestViewModelExtension : ViewModelBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        private SharedBase.Core.Version version;
        public SharedBase.Core.Version Version
        {
            get { return version; }
            set
            {
                this.RaiseAndSetIfChanged(ref version, value);
            }
        }

        private SharedBase.Core.Version minimumVersion;
        public SharedBase.Core.Version MinimumVersion
        {
            get { return minimumVersion; }
            set
            {
                this.RaiseAndSetIfChanged(ref minimumVersion, value);
            }
        }

        public ManifestViewModelExtension(SharedBase.Connection.Manifest manifest)
        {
            if (manifest != null)
            {
                Name = manifest.ClientName;
                Version = manifest.ClientVersion;
                MinimumVersion = manifest.AppMinimumVersion;
            }
        }
    }
}
