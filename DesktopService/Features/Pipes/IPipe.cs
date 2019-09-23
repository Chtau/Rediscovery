using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Pipes
{
    public interface IPipe
    {
        Task<bool> SendMessage<T>(string pipe, T message);
    }
}
