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

using Java.Interop;
using Rediscovery.Client.App.MobileAndroid.Helpers;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures.ViewModels
{
    public class FeatureViewModel : Java.Lang.Object, IParcelable
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public bool HasProfilConfiguration { get; private set; }
        public bool HasSettingConfiguration { get; private set; }

        //[ExportField("CREATOR")]
        public static Creator<FeatureViewModel> InitializeCreator()
        {
            var creator = new Creator<FeatureViewModel>();
            creator.Created += (sender, e) => e.Result = new FeatureViewModel(e.Source);
            return creator;
        }

        public FeatureViewModel(string id, string name, bool hasProfilConfiguration = false, bool hasSettingConfiguration = false)
        {
            Id = id;
            Name = name;
            HasProfilConfiguration = hasProfilConfiguration;
            HasSettingConfiguration = hasSettingConfiguration;
        }

        public FeatureViewModel(Parcel inObj)
        {
            Id = inObj.ReadString();
            Name = inObj.ReadString();
            HasProfilConfiguration = inObj.ReadInt() == 1;
            HasSettingConfiguration = inObj.ReadInt() == 1;
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(Id);
            dest.WriteString(Name);
            dest.WriteInt(HasProfilConfiguration ? 1 : 0);
            dest.WriteInt(HasSettingConfiguration ? 1 : 0);
        }
    }
}