using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.RadioButton;
using Rediscovery.Client.App.MobileAndroid.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class DeviceBottomSheetFragment : BottomSheetDialogFragment
    {
        private MaterialRadioButton rbtnThemeRedisocvery;
        private MaterialRadioButton rbtnThemeBlue;
        private MaterialRadioButton rbtnThemeGreen;
        private MaterialRadioButton rbtnThemePurple;
        private MaterialRadioButton rbtnThemeRed;
        private MaterialRadioButton rbtnThemeYellow;
        private CheckBox isFavorite;

        private ViewModels.FeatureViewModel featureViewModel;

        public event EventHandler<ViewModels.FeatureViewModel> ChangeViewModel;

        public DeviceBottomSheetFragment(ViewModels.FeatureViewModel featureViewModel)
        {
            this.featureViewModel = featureViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.device_bottom_sheet, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                isFavorite = view.FindViewById<CheckBox>(Resource.Id.checkboxFeatureFavorite);
                rbtnThemeRedisocvery = view.FindViewById<MaterialRadioButton>(Resource.Id.rbtnThemeRedisocvery);
                rbtnThemeBlue = view.FindViewById<MaterialRadioButton>(Resource.Id.rbtnThemeBlue);
                rbtnThemeGreen = view.FindViewById<MaterialRadioButton>(Resource.Id.rbtnThemeGreen);
                rbtnThemePurple = view.FindViewById<MaterialRadioButton>(Resource.Id.rbtnThemePurple);
                rbtnThemeRed = view.FindViewById<MaterialRadioButton>(Resource.Id.rbtnThemeRed);
                rbtnThemeYellow = view.FindViewById<MaterialRadioButton>(Resource.Id.rbtnThemeYellow);

                var btnOk = view.FindViewById<Button>(Resource.Id.buttonFeatureEditOk);
                btnOk.Click += (_obj, _args) => OnClose(true);
                var btnCancel = view.FindViewById<Button>(Resource.Id.buttonFeatureEditCancel);
                btnCancel.Click += (_obj, _args) => OnClose(false);

                OnLoad();
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            base.OnViewCreated(view, savedInstanceState);
        }

        private void OnLoad()
        {
            try
            {
                if (featureViewModel != null)
                {
                    OnSetSelectedTheme(Helpers.Theme.FromOrdinalEnum(featureViewModel.Feature.DisplayTheme));
                    isFavorite.Checked = featureViewModel.Feature.IsFavorite;
                } else
                {
                    // default
                    OnSetSelectedTheme(Helpers.Theme.Themes.Blue);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnClose(bool save)
        {
            try
            {
                if (save)
                {
                    featureViewModel.Feature.DisplayTheme = (int)OnGetSelectedTheme();
                    featureViewModel.Feature.IsFavorite = isFavorite.Checked;
                    Manager.DeviceManager.Instance.Save(featureViewModel.DeviceId, featureViewModel.Feature);
                    ChangeViewModel?.Invoke(this, featureViewModel);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private Theme.Themes OnGetSelectedTheme()
        {
            try
            {
                if (rbtnThemeBlue.Checked)
                    return Helpers.Theme.Themes.Blue;
                else if (rbtnThemeGreen.Checked)
                    return Helpers.Theme.Themes.Green;
                else if (rbtnThemePurple.Checked)
                    return Helpers.Theme.Themes.Purple;
                else if (rbtnThemeRed.Checked)
                    return Helpers.Theme.Themes.Red;
                else if (rbtnThemeRedisocvery.Checked)
                    return Helpers.Theme.Themes.Rediscovery;
                else if (rbtnThemeYellow.Checked)
                    return Helpers.Theme.Themes.Yellow;
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            return Helpers.Theme.Themes.Blue;
        }

        private void OnSetSelectedTheme(Theme.Themes themes)
        {
            try
            {
                switch (themes)
                {
                    case Helpers.Theme.Themes.Rediscovery:
                        rbtnThemeRedisocvery.Checked = true;
                        break;
                    case Helpers.Theme.Themes.Blue:
                        rbtnThemeBlue.Checked = true;
                        break;
                    case Helpers.Theme.Themes.Green:
                        rbtnThemeGreen.Checked = true;
                        break;
                    case Helpers.Theme.Themes.Purple:
                        rbtnThemePurple.Checked = true;
                        break;
                    case Helpers.Theme.Themes.Red:
                        rbtnThemeRed.Checked = true;
                        break;
                    case Helpers.Theme.Themes.Yellow:
                        rbtnThemeYellow.Checked = true;
                        break;
                    default:
                        rbtnThemeBlue.Checked = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}