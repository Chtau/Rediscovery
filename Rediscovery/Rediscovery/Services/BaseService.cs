using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Services
{
    public abstract class BaseService
    {
        internal SharedBase.Logging.ILogger _logger => DependencyService.Get<SharedBase.Logging.ILogger>() ?? new Logger();
    }
}
