using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Biker.Contract;
using Biker.Droid.Events;
using Biker.Droid.Services;
using Biker.Services;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationService))]
namespace Biker.Droid.Services
{
    public class LocationService : LocationServiceBase, ILocationService
    {
        public const int REQUEST_CHECK_SETTINGS = 0x1;

        private LocationManager locationManager { get; set; }

        public LocationService()
        {
            locationManager = (LocationManager)(Forms.Context.GetSystemService(Context.LocationService));
        }

        protected override bool IsDeviceLocationServiceEnabled()
        {
            try
            {
                return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override async Task<bool> EnableDeviceLocationService()
        {
            var context = Forms.Context;
            var activity = (MainActivity)context;
            var listener = new ActivityResultListener(activity);
            var googleApiClient = new GoogleApiClient.Builder(activity).AddApi(LocationServices.API).Build();
            googleApiClient.Connect();
            var locationRequest = LocationRequest.Create();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(10000);
            locationRequest.SetFastestInterval(10000 / 2);

            var builder = new LocationSettingsRequest.Builder().AddLocationRequest(locationRequest);
            builder.SetAlwaysShow(true);

            var result = LocationServices.SettingsApi.CheckLocationSettings(googleApiClient, builder.Build());

            result.SetResultCallback((LocationSettingsResult callback) =>
            {
                switch (callback.Status.StatusCode)
                {
                    case LocationSettingsStatusCodes.Success:
                        {
                            break;
                        }
                    case LocationSettingsStatusCodes.ResolutionRequired:
                        {
                            try
                            {
                                // Show the dialog by calling startResolutionForResult(), and check the result
                                // in onActivityResult().
                                callback.Status.StartResolutionForResult(activity, REQUEST_CHECK_SETTINGS);
                            }
                            catch (IntentSender.SendIntentException e)
                            {
                            }

                            break;
                        }
                    default:
                        {
                            // If all else fails, take the user to the android location settings
                            activity.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                            break;
                        }
                }
            });

            return await listener.Task;
        }

        protected override async Task<bool> ShouldRequestPermission()
        {
            return await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.LocationWhenInUse);
        }

        private class ActivityResultListener
        {
            private TaskCompletionSource<bool> Complete = new TaskCompletionSource<bool>();
            public Task<bool> Task { get { return this.Complete.Task; } }

            public ActivityResultListener(MainActivity activity)
            {
                // subscribe to activity results
                ActivityEvent.OnActivityResult += OnActivityResult;
            }

            private void OnActivityResult(object sender, Events.ActivityEvent.ActivtyResult activityResult)
            {
                // unsubscribe from activity results
                var context = Forms.Context;
                var activity = (MainActivity)context;
                Events.ActivityEvent.OnActivityResult -= OnActivityResult;

                switch (activityResult.RequestCode)
                {
                    case REQUEST_CHECK_SETTINGS:
                        {
                            switch (activityResult.ResultCode)
                            {
                                case Result.Ok:
                                    {
                                        this.Complete.TrySetResult(true);
                                        break;
                                    }
                                default:
                                    this.Complete.TrySetResult(false);
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}