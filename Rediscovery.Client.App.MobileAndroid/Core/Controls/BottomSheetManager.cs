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
    public class BottomSheetManager
    {
        public AndroidX.Fragment.App.FragmentManager SupportFragmentManager { get; }

        public BottomSheetManager(AndroidX.Fragment.App.FragmentManager supportFragmentManager)
        {
            SupportFragmentManager = supportFragmentManager;
        }

        public void Show<TModel>(BaseBottomSheet<TModel> bottomSheet, TModel viewModel, Action<TModel> successCloseCallback, string tag = null)
        {
            try
            {
                if (bottomSheet != null)
                {
                    bottomSheet.Load(viewModel);
                    bottomSheet.AfterClose += (_obj, args) =>
                    {
                        if (args != null)
                            successCloseCallback?.Invoke(args);
                    };
                    bottomSheet.Show(SupportFragmentManager, tag ?? (viewModel?.ToString() ?? Guid.NewGuid().ToSafeString()));
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}