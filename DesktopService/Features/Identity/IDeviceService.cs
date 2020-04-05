using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Identity
{
    public interface IDeviceService
    {
        event EventHandler<Models.Device> NewDeviceAdded;
        Task<Models.Device> Authenticate(string deviceName, string passwordKey);
        Task<IEnumerable<Models.Device>> GetAll();
        Task<Models.Device> GetById(Guid id);
        Task<Models.Device> GetByName(string deviceName);
        Task<Models.Device> AddDevice(string deviceName);
        string CreateNewToken(string sid, string name);
    }
}
