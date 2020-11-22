using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Service.DAL
{
    internal static class ConfigurationInstance
    {
        internal static DALConfiguration Configuration;
    }

    public class DALConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
