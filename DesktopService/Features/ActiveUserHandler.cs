using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features
{
    public static class ActiveUserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
}
