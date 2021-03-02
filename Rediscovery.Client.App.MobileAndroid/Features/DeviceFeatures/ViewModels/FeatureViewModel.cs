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
        public Models.Feature Feature { get; private set; }
        public bool HasProfilConfiguration { get; private set; }
        public bool HasSettingConfiguration { get; private set; }

        //[ExportField("CREATOR")]
        public static Creator<FeatureViewModel> InitializeCreator()
        {
            var creator = new Creator<FeatureViewModel>();
            creator.Created += (sender, e) => e.Result = new FeatureViewModel(e.Source);
            return creator;
        }

        public FeatureViewModel(Models.Feature feature)
        {
            Feature = feature ?? new Models.Feature();
            // TODO: we should load setting and profile form a special entity (e.g. FeatureMetadata) and not from the feature model
            HasProfilConfiguration = false;
            HasSettingConfiguration = false;
        }

        public FeatureViewModel(Parcel inObj)
        {
            var featureIdString = inObj.ReadString();
            var isFavoriteInt = inObj.ReadInt();
            var name = inObj.ReadString();
            var orderBy = inObj.ReadInt();
            var viewId = inObj.ReadInt();
            Feature = new Models.Feature
            {
                FeatureId = new Guid(featureIdString),
                IsFavorite = isFavoriteInt == 1,
                Name = name,
                OrderBy = orderBy,
                ViewId = viewId
            };

            HasProfilConfiguration = inObj.ReadInt() == 1;
            HasSettingConfiguration = inObj.ReadInt() == 1;
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(Feature.FeatureId.ToString());
            dest.WriteInt(Feature.IsFavorite ? 1 : 0);
            dest.WriteString(Feature.Name);
            dest.WriteInt(Feature.OrderBy);
            dest.WriteInt(Feature.ViewId);
            dest.WriteInt(HasProfilConfiguration ? 1 : 0);
            dest.WriteInt(HasSettingConfiguration ? 1 : 0);
        }
    }
}