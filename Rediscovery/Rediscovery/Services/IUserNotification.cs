using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface IUserNotification
    {
        void ShowToast(string message);
    }
}
