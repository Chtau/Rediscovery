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

        public PipeRepository(DAL.IDBContext dBContext, IPCPipe.IPipeResourceProvider resourceProvider)
        {
            _dBContext = dBContext;
            _resourceProvider = resourceProvider;
        }

        public void Init()
        {
            _resourceProvider.Provide("rediscoveryservice", OnProvideResources);
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
