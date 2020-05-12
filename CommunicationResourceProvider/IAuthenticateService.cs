using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public interface IAuthenticateService
    {
        string AuthenticationTokenRemoteResourceConsumer(string consumerKey, string roleName);
    }
}
