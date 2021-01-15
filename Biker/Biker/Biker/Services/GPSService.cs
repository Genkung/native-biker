using Biker.Contract;
using Biker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Biker.Services
{
    public class GPSService
    {
        public static async Task<bool> CanUseLocationService()
        {
            var locationService = DependencyService.Get<ILocationService>();
            return await locationService.CanUseLocationService();
        }

        public static async Task<bool> RequestLocationService() 
        {
            var locationService = DependencyService.Get<ILocationService>();
            return await locationService.RequestLocationService();
        }

        public static async Task<bool> RequestLocationPermission()
        {
            var locationService = DependencyService.Get<ILocationService>();
            return await locationService.RequestLocationPermission();
        }

        public static async Task OpenMapDirection(double lat, double lon)
        {
            try
            {
                var location = new Location(lat, lon);
                var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                await Map.OpenAsync(location, options);
            }
            catch (Exception e)
            {
                PageService.DisplayAlert("แจ้งเตือน", "ไม่สามารถเปิดแผนที่ได้ กรุณาลองใหม่อีกครัง", "ปิด");
            }
        }

        public static async Task<GeoLocationModel> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await BeginInvokeOnMainThread(() => Geolocation.GetLocationAsync(request)).Result;

                if (location != null)
                {
                    return new GeoLocationModel { Latitude = location.Longitude.ToString(), Longitude = location.Latitude.ToString() };
                }
                else return null;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                return null;
            }
            catch (PermissionException pEx)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static async Task<T> BeginInvokeOnMainThread<T>(System.Func<T> f)
        {
            var task = new TaskCompletionSource<T>();
            Device.BeginInvokeOnMainThread(() =>
            {
                task.TrySetResult(f());
            });
            return await task.Task;
        }
    }
}
