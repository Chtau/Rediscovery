using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DesktopHub.Connection.Models
{
    public class IncomingConnectionViewModel : BaseViewModel
    {
        public const string CodeArgStart = "--code:";
        public const string DeviceArgStart = "--device:";

        string code = string.Empty;
        public string Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        string device = string.Empty;
        public string Device
        {
            get { return device; }
            set { SetProperty(ref device, value); }
        }

        public IncomingConnectionViewModel(string[] args)
        {
            if (args != null)
            {
                if (args.Any(x => x.StartsWith(CodeArgStart, StringComparison.OrdinalIgnoreCase)))
                {
                    var codeArg = args.First(x => x.StartsWith(CodeArgStart, StringComparison.OrdinalIgnoreCase));
                    var vals = codeArg.Split(':');
                    Code = vals[1].Trim();
                }
                if (args.Any(x => x.StartsWith(DeviceArgStart, StringComparison.OrdinalIgnoreCase)))
                {
                    var deviceArg = args.First(x => x.StartsWith(DeviceArgStart, StringComparison.OrdinalIgnoreCase));
                    var vals = deviceArg.Split(':');
                    Device = vals[1].Trim();
                }
            }
        }
    }
}
