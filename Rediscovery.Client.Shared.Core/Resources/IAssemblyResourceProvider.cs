using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Resources
{
    public interface IAssemblyResourceProvider
    {
        string Read(Assembly assembly, string name);
    }
}
