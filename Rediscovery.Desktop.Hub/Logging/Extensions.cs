using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Logging
{
    public static class Extensions
    {
        public static SharedBase.Logging.ILogger ToSharedLogger<T>(this ILogger<T> logger)
        {
            return new Logger<T>(logger);
        }
    }
}
