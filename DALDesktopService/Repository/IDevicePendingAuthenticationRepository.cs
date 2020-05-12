using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DALDesktopService.Repository
{
    public interface IDevicePendingAuthenticationRepository
    {
        event EventHandler<Models.DevicePendingAuthentication> DevicePendingAuthenticationChanged;
        event EventHandler<Guid> DevicePendingAuthenticationDeleted;
        Task<IEnumerable<Models.DevicePendingAuthentication>> GetAll();
        Task<Models.DevicePendingAuthentication> GetById(Guid id);
        Task<Models.DevicePendingAuthentication> GetByDeviceIdentifier(string deviceIdentifier);
        Task<Models.DevicePendingAuthentication> SaveDevicePendingAuthentication(Models.DevicePendingAuthentication devicePendingAuthentication);
        Task<bool> DeleteDevicePendingAuthentication(Guid id);
    }
}
