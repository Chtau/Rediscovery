using System;
using System.Collections.Generic;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.JSONStore))]
namespace Rediscovery.Services
{
    public class JSONStore : BaseService, IJSONStore
    {
    }
}
