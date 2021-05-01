using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;

namespace Rediscovery.Client.App.MobileAndroid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        public static MainActivity Instance;

        public const int Intent_Feature_Id = 123;

        struct NavigationItem
        {
            public Guid Id { get; }
            public string Title { get; }

            public NavigationItem(string title)
            {
                Id = Guid.Empty;
                Title = title;
            }

            public NavigationItem(Guid id, string title) : this(title)
            {
                Id = id;
            }
        }

        private readonly Dictionary<int, NavigationItem> _navigationDeviceIds = new Dictionary<int, NavigationItem>();
        private Toolbar toolbar;
        private Features.DeviceFeatures.FeaturesDashboardFragment featuresDashboardFragment = null;
        private const int Menu_Dashboard_Id = 1001;
        private Core.Controls.BottomSheetManager bottomSheetManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            base.OnCreate(savedInstanceState);
            try
            {
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.activity_main);
                toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);
                bottomSheetManager = new Core.Controls.BottomSheetManager(SupportFragmentManager);

                /*FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                fab.Click += FabOnClick;*/

                DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
                drawer.AddDrawerListener(toggle);
                toggle.SyncState();
                NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                OnCreateDrawerMenuItems(navigationView);
                navigationView.SetNavigationItemSelectedListener(this);

                if (savedInstanceState == null)
                {
                    // TODO: show dashboard or last used
                    //SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, Features.DeviceFeatures.FeaturesDashboardFragment.Create()).Commit();
                }
                else
                {
                    // TODO: restore state
                }
                OnSetDefaultFragment();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (requestCode == Intent_Feature_Id)
                {
                    if (featuresDashboardFragment != null)
                        featuresDashboardFragment.UpdateFeatureGrid();
                }
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnDeviceFavoriteChanged(object sender, EventArgs args)
        {
            try
            {
                OnUpdateDrawerMenu();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnShowDeviceBottomSheet(object sender, Features.DeviceFeatures.ViewModels.FeatureViewModel featureViewModel)
        {
            try
            {
                bottomSheetManager.Show(new Features.DeviceFeatures.FeatureFavoriteSheetFragment()
                    , featureViewModel
                    , (viewModel) =>
                    {
                        try
                        {
                            // TOOD: we should check that the feature dashboard is active and is for the updated device id
                            featuresDashboardFragment.UpdateFeatureGrid();
                        }
                        catch (Exception ex)
                        {
                            Core.Logger.Instance.Error(ex);
                        }
                    }, featureViewModel.Feature.FeatureId.ToSafeString());
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnUpdateDrawerMenu()
        {
            try
            {
                NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                navigationView.Menu.Clear();
                OnCreateDrawerMenuItems(navigationView);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnCreateDrawerMenuItems(NavigationView navigationView)
        {
            try
            {
                _navigationDeviceIds.Clear();
                // default navigation items (home)
                var menuItemDashboard = navigationView.Menu.Add(0, Menu_Dashboard_Id, 0, "Dashboard");
                menuItemDashboard.SetIcon(Resource.Drawable.ic_dashboard);

                Features.Manager.DeviceManager.Instance.Init();

                var items = Features.Manager.DeviceManager.Instance.GetAll();
                if (items?.Count() > 0)
                {
                    foreach (var item in items.OrderBy(x => x.OrderBy))
                    {
                        _navigationDeviceIds.Add(item.ViewId, new NavigationItem(item.DeviceId, item.Name));
                        var menuItem = navigationView.Menu.Add(item.IsFavorite ? 1 : 2, item.ViewId, item.OrderBy, item.Name);
                        if (item.IsFavorite)
                            menuItem.SetIcon(Resource.Drawable.ic_favorite);
                        else
                            menuItem.SetIcon(Resource.Drawable.ic_device_desktop);
                    }
                }
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        /*private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }*/

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            try
            {
                int id = item.ItemId;
                if (id == Resource.Id.nav_device_add)
                {

                }
                else if (id == Resource.Id.nav_settings)
                {

                }
                /*else if (id == Menu_Dashboard_Id)
                {
                    
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, new Features.Home.DashboardFragment()).Commit();
                }*/
                else if (_navigationDeviceIds.ContainsKey(id))
                {
                    if (featuresDashboardFragment != null)
                    {
                        featuresDashboardFragment.DeviceFavoriteChanged -= OnDeviceFavoriteChanged;
                        featuresDashboardFragment.FeatureSheetRequested -= OnShowDeviceBottomSheet;
                        featuresDashboardFragment = null;
                    }
                    var navigationItem = _navigationDeviceIds[id];
                    featuresDashboardFragment = new Features.DeviceFeatures.FeaturesDashboardFragment();
                    featuresDashboardFragment.Load(navigationItem.Id);
                    featuresDashboardFragment.DeviceFavoriteChanged += OnDeviceFavoriteChanged;
                    featuresDashboardFragment.FeatureSheetRequested += OnShowDeviceBottomSheet;

                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, featuresDashboardFragment).Commit();
                    toolbar.Title = navigationItem.Title;
                }
                else
                {
                    OnSetDefaultFragment();
                }

                DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                drawer.CloseDrawer(GravityCompat.Start);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            return true;
        }

        private void OnSetDefaultFragment()
        {
            try
            {
                // TODO: show user dashboard (this dashboard should also be the default fragment on open)
                toolbar.Title = Resources.GetString(Resource.String.app_name);
                var dashboard = new Features.Home.DashboardFragment();
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, dashboard).Commit();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

