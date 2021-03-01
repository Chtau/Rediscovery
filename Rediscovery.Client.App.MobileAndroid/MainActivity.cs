using System;
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            OnCreateDrawerMenuItems(navigationView);
            navigationView.SetNavigationItemSelectedListener(this);

            if (savedInstanceState == null)
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_main_container, Features.DeviceFeatures.FeaturesDashboardFragment.Create()).Commit();
        }

        private void OnCreateDrawerMenuItems(NavigationView navigationView)
        {
            try
            {
                // TODO: test
                Core.Database.Instance.Reset();
                if (Core.Database.Instance.Get<Features.Models.Device>(x => x.Name == "D1").Count() == 0)
                {
                    Core.Database.Instance.Insert<Features.Models.Device>(new Features.Models.Device
                    {
                        DeviceId = Guid.NewGuid(),
                        Name = "D1",
                        OrderBy = 1,
                        ViewId = 1,
                        IsFavorite = true,
                        Features = new System.Collections.Generic.List<Features.Models.Feature>()
                        {
                            new Features.Models.Feature
                            {
                                FeatureId = Guid.NewGuid(),
                                Name = "F1"
                            },
                            new Features.Models.Feature
                            {
                                FeatureId = Guid.NewGuid(),
                                Name = "F2"
                            }
                        }
                    });
                }
                if (Core.Database.Instance.Get<Features.Models.Device>(x => x.Name == "D2").Count() == 0)
                {
                    Core.Database.Instance.Insert<Features.Models.Device>(new Features.Models.Device
                    {
                        DeviceId = Guid.NewGuid(),
                        Name = "D2",
                        OrderBy = 2,
                        ViewId = 2,
                        Features = new System.Collections.Generic.List<Features.Models.Feature>()
                        {
                            new Features.Models.Feature
                            {
                                FeatureId = Guid.NewGuid(),
                                Name = "F1"
                            },
                            new Features.Models.Feature
                            {
                                FeatureId = Guid.NewGuid(),
                                Name = "F2"
                            }
                        }
                    });
                }
                if (Core.Database.Instance.Get<Features.Models.Device>(x => x.Name == "D3").Count() == 0)
                {
                    Core.Database.Instance.Insert<Features.Models.Device>(new Features.Models.Device
                    {
                        DeviceId = Guid.NewGuid(),
                        Name = "D3",
                        OrderBy = 3,
                        ViewId = 3,
                        Features = new System.Collections.Generic.List<Features.Models.Feature>()
                        {
                            new Features.Models.Feature
                            {
                                FeatureId = Guid.NewGuid(),
                                Name = "F1"
                            },
                            new Features.Models.Feature
                            {
                                FeatureId = Guid.NewGuid(),
                                Name = "F2"
                            }
                        }
                    });
                }
                var items = Core.Database.Instance.GetAll<Features.Models.Device>();
                if (items?.Count() > 0)
                {
                    foreach (var item in items.OrderBy(x => x.OrderBy))
                    {
                        navigationView.Menu.Add(item.IsFavorite ? 1 : 2, 0, item.OrderBy, item.Name).SetIcon(Resource.Drawable.ic_menu_devices);
                    }
                }

                /*navigationView.Menu.Add(1, 0, 50, "Device 1").SetIcon(Resource.Drawable.ic_menu_devices);
                navigationView.Menu.Add(1, 1, 51, "Device 2").SetIcon(Resource.Drawable.ic_menu_devices);
                navigationView.Menu.Add(1, 2, 52, "Device 3").SetIcon(Resource.Drawable.ic_menu_devices);*/
            } catch (Exception ex)
            {
                Android.Util.Log.Error("", ex.ToString());
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

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

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

