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

namespace Rediscovery.Client.App.MobileAndroid.Features.Devices
{
    public class DeviceManagmentAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly LayoutInflater layoutInflater;
        private readonly Features.Models.Device _device;
        private readonly List<ViewModels.DeviceManageViewModel> _models = new List<ViewModels.DeviceManageViewModel>();
        private bool localFeatures;

        public event EventHandler<ViewModels.DeviceManageViewModel> ButtonActionClick;
        public event EventHandler<ViewModels.DeviceManageViewModel> LayoutClick;

        public DeviceManagmentAdapter(Context context, Features.Models.Device device, bool localFeatures)
        {
            this.localFeatures = localFeatures;
            _context = context;
            layoutInflater = LayoutInflater.From(context.ApplicationContext);
            _device = device;
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
                /*if (localFeatures)
                {
                    if (_device?.FeaturesLocal?.Count > 0)
                    {
                        foreach (var feature in _device.FeaturesLocal.OrderBy(x => x.OrderBy))
                        {
                            _models.Add(new ViewModels.FeatureViewModel(_device.DeviceId, feature));
                        }
                    }
                }
                else
                {
                    if (_device?.FeaturesRemote?.Count > 0)
                    {
                        foreach (var feature in _device.FeaturesRemote.OrderBy(x => x.OrderBy))
                        {
                            _models.Add(new ViewModels.FeatureViewModel(_device.DeviceId, feature));
                        }
                    }
                }*/
            }
            catch (Exception ex)
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
            return _models[position].GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            /*if (convertView == null)
            {
                convertView = layoutInflater.Inflate(Resource.Layout.item_devicefeature, parent, false);
                convertView.Tag = new FeatureViewHolder((LinearLayout)convertView);
            }
            var view = convertView;

            var holder = (FeatureViewHolder)view.Tag;
            var featureView = (ViewModels.DeviceManageViewModel)GetItem(position);
            //view.SetBackgroundColor(GetColor(theme.WindowBackgroundColor));
            holder.Title.Text = featureView.Feature.Name;
            //holder.Title.SetTextColor(GetColor((theme.TextPrimaryColor)));
            //holder.Title.SetBackgroundColor(GetColor(theme.PrimaryColor));
            if (featureView.Feature.DisplayTheme == 0)
                featureView.Feature.DisplayTheme = 1;
            var theme = Helpers.Theme.FromOrdinal(featureView.Feature.DisplayTheme, Helpers.Theme.Themes.Blue);
            OnSetIcon(featureView, holder.Icon);

            view.SetBackgroundColor(GetColor(theme.PrimaryColor));//.WindowBackgroundColor));
            holder.Title.SetTextColor(GetColor((theme.TextPrimaryColor)));
            holder.Title.SetBackgroundColor(GetColor(theme.PrimaryColor));
            holder.Button.Click += (_obj, _args) =>
            {
                ButtonActionClick?.Invoke(this, featureView);
            };
            holder.LinearLayout.Click += (_obj, _args) =>
            {
                LayoutClick?.Invoke(this, featureView);
            };*/
            return convertView;// view;
        }

        private void OnSetIcon(ViewModels.DeviceManageViewModel featureViewModel, ImageView icon)
        {
            string thumb = null;// System.IO.Path.Combine(Core.CoreIO.Instance.DeviceFeatureThumbnailDirectory(_device.DeviceId), $"{featureViewModel.Feature.FeatureId.ToSafeString()}.png");
            if (System.IO.File.Exists(thumb))
            {
                icon.SetImageURI(Android.Net.Uri.FromFile(new Java.IO.File(thumb)));
            }
            else
            {
                var featureImageResource = Resource.Drawable.icon_devicefeature_default;
                icon.SetImageResource(featureImageResource);
            }
        }

        public override bool HasStableIds => true;
        public override int Count => _models.Count;
        public override bool AreAllItemsEnabled()
        {
            return false;
        }
    }
}