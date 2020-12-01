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

        private Rediscovery.Shared.Base.Core.Version version;
        public Rediscovery.Shared.Base.Core.Version Version
        {
            get { return version; }
            set
            {
                this.RaiseAndSetIfChanged(ref version, value);
            }
        }

        private Rediscovery.Shared.Base.Core.Version minimumVersion;
        public Rediscovery.Shared.Base.Core.Version MinimumVersion
        {
            get { return minimumVersion; }
            set
            {
                this.RaiseAndSetIfChanged(ref minimumVersion, value);
            }
        }

        public ManifestViewModelExtension(Rediscovery.Shared.Base.Connection.Manifest manifest)
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
