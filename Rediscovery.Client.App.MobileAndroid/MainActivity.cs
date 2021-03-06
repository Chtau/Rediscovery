using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
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
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", MainLauncher = false)]//Theme = "@style/Rediscovery", 
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        struct NavigationItem
        {
            public Guid Id { get; }
            public string Title { get; }

            public NavigationItem(Guid id, string title)
            {
                Id = id;
                Title = title;
            }
        }

        private readonly Dictionary<int, NavigationItem> _navigationDeviceIds = new Dictionary<int, NavigationItem>();
        private Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

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
            } else
            {
                // TODO: resotre state
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
                Features.Manager.DeviceManager.Instance.Init();

                var items = Features.Manager.DeviceManager.Instance.GetAll();
                if (items?.Count() > 0)
                {
                    _navigationDeviceIds.Clear();
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
            int id = item.ItemId;

            /*if (id == Resource.Id.nav_features)
            {
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, Features.DeviceFeatures.FeaturesDashboardFragment.Create()).Commit();
            }
            else if (id == Resource.Id.nav_devices)
            {
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, Features.Home.DevicesDashboardFragment.Create()).Commit();
            }
            else if (id == Resource.Id.nav_discovery)
            {
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, Features.Home.DiscoveryFragment.Create()).Commit();
            }
            else */if (id == Resource.Id.nav_device_add)
            {

            }
            else if (id == Resource.Id.nav_settings)
            {

            } else if (_navigationDeviceIds.ContainsKey(id))
            {
                var navigationItem = _navigationDeviceIds[id];
                var featureDashboradFragment = Features.DeviceFeatures.FeaturesDashboardFragment.Create(navigationItem.Id);
                featureDashboradFragment.DeviceFavoriteChanged += (_obj, _args) =>
                {
                    OnUpdateDrawerMenu();
                };
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, featureDashboradFragment).Commit();
                toolbar.Title = navigationItem.Title;
            } else
            {
                toolbar.Title = Resources.GetString(Resource.String.app_name);
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

