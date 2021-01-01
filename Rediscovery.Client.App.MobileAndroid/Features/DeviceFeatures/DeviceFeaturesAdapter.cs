using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class DeviceFeaturesAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly LayoutInflater layoutInflater;

        private List<ViewModels.FeatureViewModel> models = new List<ViewModels.FeatureViewModel>();

        public DeviceFeaturesAdapter(Context context)
        {
            _context = context;
            layoutInflater = LayoutInflater.From(context.ApplicationContext);
            OnUpdateDatasource();
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
            OnUpdateDatasource();
        }

        private void OnUpdateDatasource()
        {
            models = new List<ViewModels.FeatureViewModel>();
            models.Add(new ViewModels.FeatureViewModel("1", "Test1"));
            models.Add(new ViewModels.FeatureViewModel("2", "Test2"));
            models.Add(new ViewModels.FeatureViewModel("3", "Test3"));
            models.Add(new ViewModels.FeatureViewModel("4", "Test4"));
            models.Add(new ViewModels.FeatureViewModel("5", "Test5"));
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return models[position];
        }

        public override long GetItemId(int position)
        {
            return models[position].Id.GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = layoutInflater.Inflate(Resource.Layout.item_devicefeature, parent, false);
                convertView.Tag = new FeatureViewHolder((LinearLayout)convertView);
            }
            var view = convertView;
            /*DeviceFeaturesAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as DeviceFeaturesAdapterViewHolder;

            if (holder == null)
            {
                holder = new DeviceFeaturesAdapterViewHolder();
                var inflater = _context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                //view = inflater.Inflate(Resource.Layout.item, parent, false);
                //holder.Title = view.FindViewById<TextView>(Resource.Id.text);
                view.Tag = holder;
            }*/


            //fill in your items
            //holder.Title.Text = "new text here";

            var holder = (FeatureViewHolder)view.Tag;
            var featureView = (FeatureViewModel)GetItem(position);
            //SetCategoryIcon(category, holder.Icon);
            //view.SetBackgroundColor(GetColor(theme.WindowBackgroundColor));
            holder.Title.Text = featureView.Name;
            //holder.Title.SetTextColor(GetColor((theme.TextPrimaryColor)));
            //holder.Title.SetBackgroundColor(GetColor(theme.PrimaryColor));

            return view;
        }

        public override bool HasStableIds => true;
        public override int Count => models.Count;
        public override bool AreAllItemsEnabled()
        {
            return false;
        }

    }

    public class DeviceFeaturesAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}