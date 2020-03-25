using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Services
{
    public abstract class BaseService
    {
        internal ILogger _logger => DependencyService.Get<ILogger>() ?? new Logger();
    }
}
