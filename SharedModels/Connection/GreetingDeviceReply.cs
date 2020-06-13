using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Connection
{
    public class GreetingDeviceReply
    {
        public string PEM { get; set; }
        public SharedBase.Connection.Enums.AllowConnect CanConnect { get; set; }
    }
}
