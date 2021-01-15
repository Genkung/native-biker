using Biker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Biker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationPermissionPage : ContentPage
    {
        public LocationPermissionPage()
        {
            InitializeComponent();
            Events.AppEvent.OnAppResume += OnAppResume;
            permissionBtn.Clicked += (s, e) =>
            {
                CheckPermissionAndGPS();
            };
            CheckPermission();
        }

        private async void CheckPermissionAndGPS() 
        {
            var canUseLocationService = await GPSService.RequestLocationService();
            if (canUseLocationService)
            {
                App.Current.MainPage = new LoginPage();
            }
        }

        private async void CheckPermission()
        {
            var canUseLocationService = await GPSService.CanUseLocationService();
            if (canUseLocationService)
            {
                App.Current.MainPage = new LoginPage();
            }
            else
            {
                await GPSService.RequestLocationPermission();
                canUseLocationService = await GPSService.CanUseLocationService();
                if (canUseLocationService)
                {
                    App.Current.MainPage = new LoginPage();
                }
            }
        }

        private async void OnAppResume(object sender, Page e)
        {
            var canUseLocationService = await GPSService.CanUseLocationService();
            if (canUseLocationService)
            {
                App.Current.MainPage = new LoginPage();
            }
        }
    }
}