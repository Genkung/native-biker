using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Biker.Services
{
    public abstract class LocationServiceBase
    {
        private Location location;

        public LocationServiceBase()
        {
        }

        protected abstract bool IsDeviceLocationServiceEnabled();
        protected abstract Task<bool> EnableDeviceLocationService();
        protected abstract Task<bool> ShouldRequestPermission();

        public async Task<bool> CanUseLocationService()
        {
            var deviceLocationServiceEnabled = IsDeviceLocationServiceEnabled();
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            var locationPermissionGranted = locationPermissionStatus == PermissionStatus.Granted;

            return locationPermissionGranted && deviceLocationServiceEnabled;
        }
        public async Task<bool> RequestLocationService()
        {
            var deviceLocationServiceEnabled = await requestDeviceLocationService();
            if (!deviceLocationServiceEnabled) return false;
            var locationPermissionGranted = await requestLocationPermission();

            return locationPermissionGranted && deviceLocationServiceEnabled;
        }
        public async Task<bool> RequestLocationPermission()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            var appLocationPermissionGranted = locationPermissionStatus == PermissionStatus.Granted;
            if (!appLocationPermissionGranted)
            {
                var result = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                return result == PermissionStatus.Granted;
            }
            else
            {
                return false;
            }
        }

        public async Task<Location> GetCurrentLocation(bool fresh = false)
        {
            var shouldReload = null == location || fresh;
            if (shouldReload)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                location = await Geolocation.GetLocationAsync(request);
            }
            return location;
        }

        private async Task<bool> requestLocationPermission()
        {
            var shouldRequestPermission = await ShouldRequestPermission();
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            var appLocationPermissionDenied = locationPermissionStatus != PermissionStatus.Granted;

            if (shouldRequestPermission)
            {
                var result = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                return result == PermissionStatus.Granted;
            }
            else
            {
                if (appLocationPermissionDenied)
                {
                    CrossPermissions.Current.OpenAppSettings();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        private async Task<bool> requestDeviceLocationService()
        {
            try
            {
                var deviceLocationServiceEnabled = IsDeviceLocationServiceEnabled();
                if (deviceLocationServiceEnabled)
                {
                    return true;
                }
                else
                {
                    return await EnableDeviceLocationService();
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
