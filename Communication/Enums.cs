using System;
using System.Collections.Generic;
using System.Text;

namespace Communication
{
	public enum Protocol
	{
		HTTP,
		HTTPS
	}

    public enum ConnectionState
    {
        None = 0,
        OK = 1,
        Error = 2,
        Warning = 3,
        Offline = 4,
        Denied = 5,
        WaitForApprovel = 6
    }
}
