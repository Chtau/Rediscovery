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
    public interface IBaseBottomSheet<TModel>
    {
        TModel ViewModel { get; }
        event EventHandler<TModel> AfterClose;
        void Load(TModel viewModel);
        void Show(AndroidX.Fragment.App.FragmentManager manager, string tag);
    }
}