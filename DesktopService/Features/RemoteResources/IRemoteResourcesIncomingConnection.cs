using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.RemoteResources
{
    public interface IRemoteResourcesIncomingConnection
    {
        [Obsolete("Replace with new Device accept logic")]
        Task ShowCode(string code, string device, DateTime validTill);
    }
}
