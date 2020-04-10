using DesktopService.Features.Identity.Models;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Pipes
{
    public class PipeRepository : IPipeRepository
    {
        private readonly DAL.IDBContext _dBContext;
        private readonly DeviceFeature.IFeatureService _featureService;
        private readonly IPCPipe.IPipeResourceProvider _resourceProvider;
        private readonly IPCPipe.IPipeServer _pipeServer;

        public PipeRepository(DAL.IDBContext dBContext, IPCPipe.IPipeResourceProvider resourceProvider,
            IPCPipe.IPipeServer pipeServer, DeviceFeature.IFeatureService featureService)
        {
            _dBContext = dBContext;
            _resourceProvider = resourceProvider;
            _pipeServer = pipeServer;
            _featureService = featureService;
        }

        public void Init()
        {
            _resourceProvider.Provide("rediscoveryservice", OnProvideResources);
            _pipeServer.DataReceived += _pipeServer_DataReceived;
            _pipeServer.Listen("sync_device_rediscoveryservice");
            Task.Run(OnSendHeartbeat);
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
                var users = _dBContext.Instance.Table<Device>().ToListAsync().GetAwaiter().GetResult();
                var resource = new IPCPipe.Models.PipeResource<List<SharedCoreModels.DeviceInfo>>();
                resource.ResourceName = resourceName;
                resource.Entity = (from x in users
                                  select new SharedCoreModels.DeviceInfo
                                  {
                                      Id = x.Id,
                                      AllowAccess = x.AllowAccess,
                                      Name = x.DeviceName
                                  }).ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(resource);
            } else if (resourceName == "features")
            {
                var features = _featureService.GetFeaturesManifest();
                var resource = new IPCPipe.Models.PipeResource<List<DeviceFeatureDefinition>>();
                resource.ResourceName = resourceName;
                resource.Entity = features;
                return Newtonsoft.Json.JsonConvert.SerializeObject(resource);
            }
            return null;
        }

        private async Task OnSendHeartbeat()
        {
            do
            {
                try
                {
                    await Task.Delay(1000);
                    Console.WriteLine($"TODO: Heartbeat");
                    _pipeServer.Send("rediscoveryheartbeathub", DateTime.Now.ToString());
                } catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            } while (true);
        }
    }
}
