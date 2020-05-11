using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public interface IAuthenticateService
    {
        string AuthenticateRemoteResourceConsumer(string consumerKey, string roleName);
    }
}
