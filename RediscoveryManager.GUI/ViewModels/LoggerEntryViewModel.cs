using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.ViewModels
{
    public class LoggerEntryViewModel : ViewModelBase
    {
        public Rediscovery.Shared.Logging.LoggerType LogLevel { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public string Module { get; set; }

        public DateTime Time { get; set; }

        public string Sid { get; set; }
    }
}
