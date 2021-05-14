using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Rediscovery.Client.App.MobileAndroid.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.Devices.ViewModels
{
    public class HostManageViewModel : Java.Lang.Object, IParcelable
    {
        public string FriendlyName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; } = -1;

        public static Creator<HostManageViewModel> InitializeCreator()
        {
            var creator = new Creator<HostManageViewModel>();
            creator.Created += (sender, e) => e.Result = new HostManageViewModel(e.Source);
            return creator;
        }

        public HostManageViewModel(string friendlyName, string ip, int port)
        {
            FriendlyName = friendlyName;
            IP = ip;
            Port = port;
        }

        public HostManageViewModel(Parcel inObj)
        {
            FriendlyName = inObj.ReadString();
            IP = inObj.ReadString();
            Port = inObj.ReadInt();
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(FriendlyName);
            dest.WriteString(IP);
            dest.WriteInt(Port);            
        }
    }
}