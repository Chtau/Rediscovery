using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public class IncomingConnectionInfo
    {
        public string Code { get; set; }
        public string Device { get; set; }
        public DateTime Created { get; set; }
    }
}
