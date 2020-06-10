using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Connection
{
    public class IncomingConnectionInfo
    {
        public string Code { get; set; }
        public string Device { get; set; }
        public DateTime ValidTill { get; set; }
    }
}
