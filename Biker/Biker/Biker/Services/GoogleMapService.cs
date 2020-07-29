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
            try
            {
                var location = new Location(lat, lon);
                var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                await Map.OpenAsync(location, options);
            }
            catch (Exception e)
            {
                await PageService.DisplayAlert("แจ้งเตือน", "ไม่สามารถเปิดแผนที่ได้ กรุราลองใหม่อีกครัง", "ปิด");
            }
        }
    }
}
