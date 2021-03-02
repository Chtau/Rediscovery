using Android.App;
using Android.Content;
using Android.Graphics;
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
        private readonly Guid _deviceId;
        private readonly List<ViewModels.FeatureViewModel> _models = new List<ViewModels.FeatureViewModel>();

        public DeviceFeaturesAdapter(Context context, Guid deviceId)
        {
            _context = context;
            layoutInflater = LayoutInflater.From(context.ApplicationContext);
            _deviceId = deviceId;
            OnUpdateDatasource();
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
            OnUpdateDatasource();
        }

        private void OnUpdateDatasource()
        {
            try
            {
                _models.Clear();
                var device = Core.Database.Instance.Get<Features.Models.Device>(x => x.DeviceId == _deviceId).FirstOrDefault();
                if (device?.Features?.Count > 0)
                {
                    foreach (var feature in device.Features.OrderBy(x => x.OrderBy))
                    {
                        _models.Add(new ViewModels.FeatureViewModel(feature.FeatureId.ToString(), feature.Name));
                    }
                }
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return _models[position];
        }

        public override long GetItemId(int position)
        {
            return _models[position].Id.GetHashCode();
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
            //view.SetBackgroundColor(GetColor(theme.WindowBackgroundColor));
            holder.Title.Text = featureView.Name;
            //holder.Title.SetTextColor(GetColor((theme.TextPrimaryColor)));
            //holder.Title.SetBackgroundColor(GetColor(theme.PrimaryColor));
            var theme = Helpers.Theme.Rediscovery;
            OnSetIcon(featureView, holder.Icon);
            view.SetBackgroundColor(GetColor(theme.WindowBackgroundColor));
            holder.Title.SetTextColor(GetColor((theme.TextPrimaryColor)));
            holder.Title.SetBackgroundColor(GetColor(theme.PrimaryColor));
            return view;
        }

        private void OnSetIcon(FeatureViewModel featureViewModel, ImageView icon)
        {
            var featureImageResource = Resource.Drawable.icon_devicefeature_default; //_context.GetIdentifier(IconCategory + category.Id, Drawable, packageName);
            icon.SetImageResource(featureImageResource);
            /*var solved = category.Solved;
            if (solved)
            {
                var solvedIcon = LoadSolvedIcon(category, categoryImageResource);
                icon.SetImageDrawable(solvedIcon);
            }
            else
            {
                icon.SetImageResource(categoryImageResource);
            }*/
        }

        private Color GetColor(int colorRes)
        {
            return new Color(_context.GetColor(colorRes));
        }

        public override bool HasStableIds => true;
        public override int Count => _models.Count;
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