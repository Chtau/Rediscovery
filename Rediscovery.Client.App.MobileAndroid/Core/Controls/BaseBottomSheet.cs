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

namespace Rediscovery.Client.App.MobileAndroid.Core.Controls
{
    public abstract class BaseBottomSheet<TModel> : BottomSheetDialogFragment
    {
        public TModel ViewModel { get; private set; }

        public event EventHandler<TModel> AfterClose;

        internal virtual int Layout { get; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Layout, container, false);
        }

        public virtual void Load(TModel viewModel)
        {
            ViewModel = viewModel;
        }

        internal void OnInvokeAfterClose(TModel model)
        {
            try
            {
                AfterClose?.Invoke(this, model);
                Dismiss();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}