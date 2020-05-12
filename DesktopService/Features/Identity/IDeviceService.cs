using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Identity
{
    public interface IDeviceService
    {
        [Obsolete("Use Library DALDesktopService")]
        event EventHandler<Models.Device> NewDeviceAdded;
        Task<Models.Device> Authenticate(string deviceName, string passwordKey);
        [Obsolete("Use Library DALDesktopService")]
        Task<IEnumerable<Models.Device>> GetAll();
        [Obsolete("Use Library DALDesktopService")]
        Task<Models.Device> GetById(Guid id);
        [Obsolete("Use Library DALDesktopService")]
        Task<Models.Device> GetByName(string deviceName);
        [Obsolete("Use Library DALDesktopService")]
        Task<Models.Device> AddDevice(string deviceName);
        string CreateNewToken(string sid, string name);
        string AuthenticateRemoteResourceConsumer(string consumerKey);


        [Obsolete("Use Library DALDesktopService")]
        event EventHandler<Models.DevicePendingAuthentication> NewDevicePendingAuthenticationAdded;
        [Obsolete("Use Library DALDesktopService")]
        Task<Models.DevicePendingAuthentication> AddPendingAuthentication(string deviceName, string deviceIdentifier);
        [Obsolete("Use Library DALDesktopService")]
        Task<Models.DevicePendingAuthentication> PendingAuthenticationByIdentifier(string deviceIdentifier);
    }
}
