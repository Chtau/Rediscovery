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
    public class DeviceManageViewModel : Java.Lang.Object, IParcelable
    {
        public string IP { get; set; }
        public int Port { get; set; } = -1;

        public static Creator<DeviceManageViewModel> InitializeCreator()
        {
            var creator = new Creator<DeviceManageViewModel>();
            creator.Created += (sender, e) => e.Result = new DeviceManageViewModel(e.Source);
            return creator;
        }

        public DeviceManageViewModel(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        public DeviceManageViewModel(Parcel inObj)
        {
            IP = inObj.ReadString();
            Port = inObj.ReadInt();
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(IP);
            dest.WriteInt(Port);            
        }
    }
}