using Rediscovery.Desktop.Hub.Feature.RemoteResource;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly IDesktopHubRemoteResourceService _desktopHubRemoteResourceService;
        public event EventHandler<LoggerEntryModel> LoggerDataReceived;

        public LoggerService(IDesktopHubRemoteResourceService desktopHubRemoteResourceService)
        {
            _desktopHubRemoteResourceService = desktopHubRemoteResourceService;
            _desktopHubRemoteResourceService.LogEntryReceived += _desktopHubRemoteResourceService_LogEntryReceived;
        }

        private void _desktopHubRemoteResourceService_LogEntryReceived(object sender, LoggerEntryModel e)
        {
            LoggerDataReceived?.Invoke(this, e);
        }

        public void Init()
        {
            _desktopHubRemoteResourceService.Connect();
        }
    }
}
