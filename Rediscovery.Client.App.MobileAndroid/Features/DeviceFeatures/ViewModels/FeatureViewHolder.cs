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

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures.ViewModels
{
    public class FeatureViewHolder : Java.Lang.Object
    {
		public TextView Title { get; set; }

		public ImageView Icon { get; set; }

		public FeatureViewHolder(LinearLayout container)
		{
			Icon = container.FindViewById<ImageView>(Resource.Id.feature_icon);
			Title = container.FindViewById<TextView>(Resource.Id.feature_title);
		}
	}
}