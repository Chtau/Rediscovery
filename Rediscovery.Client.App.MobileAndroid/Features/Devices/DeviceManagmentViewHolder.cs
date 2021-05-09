using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.Devices
{
    public class DeviceManagmentViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; private set; }

        public DeviceManagmentViewHolder(View itemView) : base(itemView)
        {
            // Locate and cache view references:
            Title = itemView.FindViewById<TextView>(Resource.Id.device_managment_title);
        }
    }
}