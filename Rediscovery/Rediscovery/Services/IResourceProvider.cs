using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface IResourceProvider
    {
        string ReadResource(string name);
    }
}
