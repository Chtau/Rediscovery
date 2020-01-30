using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Services
{
    public interface ILogger
    {
        void Exception(Exception ex, string module = null);
        void Warning(string msg, string module = null);
        void Info(string msg, string module = null);
        void Diagnostic(string msg, string module = null);
    }
}
