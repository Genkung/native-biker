using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Biker.Services
{
    public class GoogleMapService
    {
        public static async Task OpenMapDirection(double lat, double lon)
        {
            var location = new Location(lat, lon);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

            await Map.OpenAsync(location, options);
        }
    }
}
