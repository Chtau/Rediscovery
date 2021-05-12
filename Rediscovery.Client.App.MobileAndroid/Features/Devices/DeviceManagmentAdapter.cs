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
    public class DeviceManagmentAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly List<ViewModels.DeviceManageViewModel> _models = new List<ViewModels.DeviceManageViewModel>();

        public event EventHandler<ViewModels.DeviceManageViewModel> EditModel;
        public event EventHandler<ViewModels.DeviceManageViewModel> ConnectModel;

        public override int ItemCount => _models.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            DeviceManagmentViewHolder vh = holder as DeviceManagmentViewHolder;
            vh.Title.Text = _models[position].Title;
            vh.Subtitle.Text = $"{_models[position].IP}:{_models[position].Port}";
            vh.Edit.Click += (obj, args) =>
            {
                EditModel?.Invoke(this, _models[position]);
            };
            vh.Connect.Click += (obj, args) =>
            {
                ConnectModel?.Invoke(this, _models[position]);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_device_managment, parent, false);

            // Create a ViewHolder to hold view references inside the CardView:
            DeviceManagmentViewHolder vh = new DeviceManagmentViewHolder(itemView);
            return vh;
        }

        public DeviceManagmentAdapter(Context context)
        {
            _context = context;
            OnUpdateDatasource();
        }

        public void Refresh()
        {
            OnUpdateDatasource();
        }

        private void OnUpdateDatasource()
        {
            try
            {
                _models.Clear();
                for (int i = 0; i < 5; i++)
                {
                    _models.Add(new ViewModels.DeviceManageViewModel($"Title {i + 1}", $"127.0.0.{i + 1}", 80));
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        /*private readonly Context _context;
        private readonly LayoutInflater layoutInflater;
        private readonly Features.Models.Device _device;
        
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
        }*/
    }
}