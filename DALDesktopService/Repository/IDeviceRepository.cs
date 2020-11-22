using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Service.DAL.Repository
{
    public interface IDeviceRepository
    {
        event EventHandler<Models.Device> DeviceChanged;
        event EventHandler<Guid> DeviceDeleted;
        Task<IEnumerable<Models.Device>> GetAll();
        Task<Models.Device> GetById(Guid id);
        Task<Models.Device> GetByDeviceIdentifier(string deviceIdentifier);
        Task<Models.Device> SaveDevice(Models.Device device);
        Task<bool> DeleteDevice(Guid id);
    }
}
