﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Biker.Services;
using Biker.Views;

namespace Biker.Droid
{
    [Activity(Label = "Biker", Icon = "@drawable/icon", Theme = "@style/Biker.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            Xamarin.Forms.MessagingCenter.Subscribe<MainPage, string>(this, Models.MessagingChannel.HomeReady, (sub, msg) =>
            {
                var toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);
            });
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (PageService.GetCurrentPage() is MainPage)
            {
                ((Xamarin.Forms.MasterDetailPage)Xamarin.Forms.Application.Current.MainPage).IsPresented = true;
                return base.OnOptionsItemSelected(item);
            }

            if (item.ItemId == 16908332)
            {
                var page = PageService.GetCurrentPage() as WebViewPage;
                page.XamarinGoBack();
                return false;
            }
            else
            {
                return base.OnOptionsItemSelected(item);
            }
        }

        public override void OnBackPressed()
        {
            if (PageService.GetCurrentPage() is WebViewPage page)
            {
                page.XamarinGoBack();
            }
            else
            {
                base.OnBackPressed();
            }
        }
    }


    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { "android.intent.action.VIEW" }, Categories = new[] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" }, DataScheme = "mauth")]
    public class WebAuthenticationCallbackActivity : Xamarin.Essentials.WebAuthenticatorCallbackActivity
    {
    }
}