using DesktopService.Features.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopService.Features.Pipes
{
    public class PipeRepository : IPipeRepository
    {
        private readonly DAL.IDBContext _dBContext;
        private readonly IPCPipe.IPipeResourceProvider _resourceProvider;
        private readonly IPCPipe.IPipeServer _pipeServer;

        public PipeRepository(DAL.IDBContext dBContext, IPCPipe.IPipeResourceProvider resourceProvider,
            IPCPipe.IPipeServer pipeServer)
        {
            _dBContext = dBContext;
            _resourceProvider = resourceProvider;
            _pipeServer = pipeServer;
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
            }
            return null;
        }
    }
}
