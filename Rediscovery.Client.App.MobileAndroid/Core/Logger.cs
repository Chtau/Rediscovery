using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Core
{
    public sealed class Logger
    {
        private Logger()
        {
        }

        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());

        public static Logger Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public void Error(Exception ex, string sourceTag = null)
        {
            Android.Util.Log.Error(string.IsNullOrWhiteSpace(sourceTag) ? ex?.Source : sourceTag, ex?.ToString());
        }

        public void Warning(Exception ex, string sourceTag = null)
        {
            Android.Util.Log.Warn(string.IsNullOrWhiteSpace(sourceTag) ? ex?.Source : sourceTag, ex?.ToString());
        }

        public void Warning(string message, string sourceTag = null)
        {
            Android.Util.Log.Warn(string.IsNullOrWhiteSpace(sourceTag) ? nameof(Warning) : sourceTag, message);
        }

        public void Debug(Exception ex, string sourceTag = null)
        {
            Android.Util.Log.Debug(string.IsNullOrWhiteSpace(sourceTag) ? ex?.Source : sourceTag, ex?.ToString());
        }

        public void Debug(string message, string sourceTag = null)
        {
            Android.Util.Log.Debug(string.IsNullOrWhiteSpace(sourceTag) ? nameof(Debug) : sourceTag, message);
        }
    }
}