using DesktopService.Features.DeviceFeature;
using DesktopService.Features.Identity.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.RemoteResources
{
    public class RemoteResourcesRepository : IRemoteResourcesRepository
    {
        private readonly DAL.IDBContext _dBContext;
        private readonly DeviceFeature.IFeatureService _featureService;
        private readonly IPCPipe.IPipeResourceProvider _resourceProvider;
        private readonly IPCPipe.IPipeServer _pipeServer;
        private readonly IPCPipe.IPipeClient _pipeClient;

        private readonly ILogger<RemoteResourcesRepository> _logger;

        public RemoteResourcesRepository(DAL.IDBContext dBContext, IPCPipe.IPipeResourceProvider resourceProvider,
            IPCPipe.IPipeServer pipeServer, DeviceFeature.IFeatureService featureService,
            IPCPipe.IPipeClient pipeClient,
            ILoggerFactory loggerFactory)
        {
            _dBContext = dBContext;
            _resourceProvider = resourceProvider;
            _pipeServer = pipeServer;
            _pipeClient = pipeClient;
            _featureService = featureService;
            _logger = loggerFactory.CreateLogger<RemoteResourcesRepository>();
        }

        public void Init()
        {
            _resourceProvider.Provide("rediscoveryservice", OnProvideResources);
            _pipeServer.DataReceived += _pipeServer_DataReceived;
            _pipeServer.Listen("sync_device_rediscoveryservice");
        }

        private void _pipeServer_DataReceived(object sender, string e)
        {
            if (!string.IsNullOrWhiteSpace(e))
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<IPCPipe.Models.Sync<SharedCoreModels.DeviceInfo>>(e);
                if (item != null)
                {
                    switch (item.ActionType)
                    {
                        case IPCPipe.Models.SyncAction.None:
                            break;
                        case IPCPipe.Models.SyncAction.Add:
                            break;
                        case IPCPipe.Models.SyncAction.Delete:
                            _dBContext.Instance.Table<Device>().DeleteAsync(x => x.Id == item.Entity.Id);
                            break;
                        case IPCPipe.Models.SyncAction.Update:
                            break;
                    }
                }
            }
        }

        private string OnProvideResources(string resourceName)
        {
            if (resourceName == "deviceinfo")
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(OnGetResourceDeviceInfo());
            } else if (resourceName == "features")
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(OnGetResourceDeviceFeature());
            }
            else if (resourceName == "activedeviceinfo")
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(OnGetResourceActiveDeviceInfo());
            }
            return null;
        }

        private IPCPipe.Models.PipeResource<List<SharedCoreModels.DeviceFeature>> OnGetResourceDeviceFeature()
        {
            var features = _featureService.GetFeaturesManifest();
            var resource = new IPCPipe.Models.PipeResource<List<SharedCoreModels.DeviceFeature>>();
            resource.ResourceName = "features";
            resource.Entity = (from x in features
                               select new SharedCoreModels.DeviceFeature
                               {
                                   Id = x.Id,
                                   DisplayName = x.DisplayName,
                                   MinControlIntegrationPoint = x.MinControlIntegrationPoint.ToString(),
                                   MinFeatureIntegrationPoint = x.MinFeatureIntegrationPoint.ToString(),
                                   Version = x.Version.ToString()
                               }).ToList();
            return resource;
        }

        private IPCPipe.Models.PipeResource<List<SharedCoreModels.DeviceInfo>> OnGetResourceDeviceInfo()
        {
            var users = _dBContext.Instance.Table<Device>().ToListAsync().GetAwaiter().GetResult();
            var resource = new IPCPipe.Models.PipeResource<List<SharedCoreModels.DeviceInfo>>();
            resource.ResourceName = "deviceinfo";
            resource.Entity = (from x in users
                               select new SharedCoreModels.DeviceInfo
                               {
                                   Id = x.Id,
                                   AllowAccess = x.AllowAccess,
                                   Name = x.DeviceName
                               }).ToList();
            return resource;
        }

        private IPCPipe.Models.PipeResource<List<SharedCoreModels.DeviceInfo>> OnGetResourceActiveDeviceInfo()
        {
            var allUsers = from x in ActiveUserHandler.UserIds select new Guid(x);
            var users = _dBContext.Instance.Table<Device>().ToListAsync().GetAwaiter().GetResult();
            var resource = new IPCPipe.Models.PipeResource<List<SharedCoreModels.DeviceInfo>>();
            resource.ResourceName = "activedeviceinfo";
            resource.Entity = (from x in users
                               join y in allUsers on x.Id equals y
                               select new SharedCoreModels.DeviceInfo
                               {
                                   Id = x.Id,
                                   AllowAccess = x.AllowAccess,
                                   Name = x.DeviceName
                               }).ToList();
            return resource;
        }

        public void ActiveDeviceInfoChanged()
        {
            try
            {
                _pipeClient.Send("rediscoveryserviceresourcechanged", "activedeviceinfo");
            } catch (Exception ex)
            {
                _logger.LogError(ex, "ActiveDeviceInfoChanged IPC");
            }
        }

        public void DeviceInfoChanged()
        {
            try
            {
                _pipeClient.Send("rediscoveryserviceresourcechanged", "deviceinfo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeviceInfoChanged IPC");
            }
        }

        public void FeatureChanged()
        {
            try
            {
                _pipeClient.Send("rediscoveryserviceresourcechanged", "features");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FeatureChanged IPC");
            }
        }
    }
}
