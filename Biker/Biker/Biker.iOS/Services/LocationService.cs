using Biker.Contract;
using Biker.iOS.Services;
using Biker.Services;
using CoreLocation;
using Foundation;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationService))]
namespace Biker.iOS.Services
{
    public class LocationService : LocationServiceBase, ILocationService
    {
        protected override bool IsDeviceLocationServiceEnabled()
        {
            return CLLocationManager.LocationServicesEnabled;
        }

        protected override async Task<bool> EnableDeviceLocationService()
        {
            var shouldOpenAppSetting = await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("บริการหาตำแหน่งที่ตั้งยังไม่ได้เปิดใช้งานในขณะนี้", "ไปที่ การตั้งค่า > ความเป็นส่วนตัว > เปิดใช้งาน บริการหาตำแหน่งที่ตั้ง", "การตั้งค่า", "ย้อนกลับ");
            if (shouldOpenAppSetting)
            {
                CrossPermissions.Current.OpenAppSettings();
            }

            return false;
        }

        protected override async Task<bool> ShouldRequestPermission()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            return locationPermissionStatus != PermissionStatus.Denied && locationPermissionStatus != PermissionStatus.Granted;
        }
    }
}