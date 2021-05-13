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
    public class BottomSheetEventArgs<TSheet, TModel> : EventArgs where TSheet : IBaseBottomSheet<TModel>
    {
        public TSheet BottomSheet { get; }
        public TModel ViewModel { get; }

        public BottomSheetEventArgs(TSheet bottomSheet, TModel viewModel) : base()
        {
            BottomSheet = bottomSheet;
            ViewModel = viewModel;
        }
    }
}