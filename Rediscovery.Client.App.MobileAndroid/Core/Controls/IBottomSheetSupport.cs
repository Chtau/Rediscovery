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

namespace Rediscovery.Client.App.MobileAndroid.Core.Controls
{
    public interface IBottomSheetSupport<TSheet, TModel> where TSheet : IBaseBottomSheet<TModel>
    {
        public event EventHandler<BottomSheetEventArgs<TSheet, TModel>> OpenSheet;
        public void AfterCloseSheet(TModel viewModel);
    }
}