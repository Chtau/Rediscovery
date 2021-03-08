using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class DeviceBottomSheetFragment : BottomSheetDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.device_bottom_sheet, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            /*var listView = view.FindViewById<ListView>(Resource.Id.listViewOptions);
            listView.Adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, new List<string>
            {
                "Share with Friends",
                "Bookmark",
                "Add to Favourites",
                "More Information"
            });*/
            //featureGridView = view.FindViewById<GridView>(Resource.Id.devicefeatures);
            //OnSetUpFeatures(featureGridView);
            base.OnViewCreated(view, savedInstanceState);
        }
    }
}