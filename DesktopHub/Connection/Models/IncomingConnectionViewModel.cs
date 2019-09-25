using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopHub.Connection.Models
{
    public class IncomingConnectionViewModel : BaseViewModel
    {
        public const string CodeArgStart = "--code:";
        public const string DeviceArgStart = "--device:";
        public const string ValidArgStart = "--valid:";

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

        string countdown = "00:00 minutes";
        public string Countdown
        {
            get { return countdown; }
            set { SetProperty(ref countdown, value); }
        }

        private DateTime validTill = DateTime.Now.AddMinutes(5);

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
                if (args.Any(x => x.StartsWith(ValidArgStart, StringComparison.OrdinalIgnoreCase)))
                {
                    var validArg = args.First(x => x.StartsWith(ValidArgStart, StringComparison.OrdinalIgnoreCase));
                    var vals = validArg.Split(':');
                    var val = vals[1].Trim();
                    if (long.TryParse(val, out long result))
                        validTill = new DateTime(result);
                }
                OnCountdown();
            }
        }

        public void InitCountdown(DateTime validTill)
        {
            this.validTill = validTill;
            OnCountdown();
        }

        private void OnCountdown()
        {
            Task.Run(async () =>
            {
                while (DateTime.UtcNow < validTill)
                {
                    var dif = validTill - DateTime.UtcNow;
                    Countdown = $"{dif.Minutes.ToString("00")}:{dif.Seconds.ToString("00")} minutes";
                    await Task.Delay(1000);
                }
                Countdown = "Request a new Key on the Mobile App";
            });
        }
    }
}
