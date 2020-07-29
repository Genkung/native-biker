﻿using Biker.Models;
using Biker.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TheS.DevXP.XamForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Biker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage(string pageName, object parameters)
        {
            InitializeComponent();

            myWebview.Accessors = new TheS.DevXP.XamForms.XWebViewAccessorCollection(
                LocalContentAccessor.GetAppData(WebviewService.MCLocalStorageFolderName));

            var htmlSource = WebviewService.GetHtmlPathByName(pageName);

            RegisterWebviewBaseFunction();

            myWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
        }

        private void RegisterWebviewBaseFunction()
        {
            myWebview.RegisterNativeFunction("NavigateToPage", NavigateToPage);
            myWebview.RegisterNativeFunction("GetBikerId", GetBikerId);
            myWebview.RegisterNativeFunction("GetTokenIfExist", GetTokenIfExist);
            myWebview.RegisterCallback("Logout", Logout);
            myWebview.RegisterCallback("Goback", Goback);
            myWebview.RegisterCallback("PopToRoot", PopToRoot);
            myWebview.RegisterCallback("SetPageTitle", SetPageTitle);
            myWebview.RegisterCallback("ExecuteNotiIfExist", ExecuteNotiIfExist);
            myWebview.RegisterCallback("RemoveNotificationChannel", RemoveNotificationChannel);
            myWebview.RegisterCallback("OpenMapDirection", OpenMapDirection);
            myWebview.RegisterCallback("PhoneCall", PhoneCall);
            myWebview.RegisterCallback("UpdateSidemenuItem", UpdateSidemenuItem);
        }

        private async Task<object[]> NavigateToPage(string param)
        {
            return new object[] { false };
        }

        private async Task<object[]> GetBikerId(string param)
        {
            var biker = BikerService.GetBikerInfo();
            return new object[] { biker._id };
        }

        private async Task<object[]> GetTokenIfExist(string param)
        {
            var hasExpired = AuthService.HasExpired;
            var token = hasExpired ? "" : AuthService.GetAccessToken();
            var result = new { HasExpired = hasExpired, Token = token };
            return new object[] { result };
        }

        private async void Logout(string param)
        {
            await AuthService.Logout();
        }

        private async void Goback(string param)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                GoBack();
            });
        }

        private async void PopToRoot(string param)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopAsync(true);
            });
        }


        private async void SetPageTitle(string title)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Title = title;
            });
        }

        private async void ExecuteNotiIfExist(string notiChannel)
        {
            NotificationService.ExecuteNotificationIfExist(notiChannel);
        }

        private async void RemoveNotificationChannel(string notiChannel)
        {
            NotificationService.RemoveNotificationStack(notiChannel);
        }

        private async void OpenMapDirection(string directionParam)
        {
            var latLon = JsonConvert.DeserializeObject<OpenDirectionParam>(directionParam);
            await GoogleMapService.OpenMapDirection(latLon.Latitude, latLon.Longitude);
        }

        private async void PhoneCall(string phoneNumber)
        {
            PhoneService.Call(phoneNumber);
        }

        private async void UpdateSidemenuItem(string param)
        {
            var sidemenu = JsonConvert.DeserializeObject<SideMenuItem>(param);
            SidemenuService.UpdateSidemenuPage(sidemenu.Title, sidemenu.Page, sidemenu.Params);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            myWebview.Focus();

            NotificationService.SubscriptNotification((sender, obj) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await myWebview?.EvaluateJavaScriptAsync($"onSendNotification('{obj.NotiChannel}',{obj.Params});");
                });
            });

            myWebview.EvaluateJavaScriptAsync("refreshOnGoBack();");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NotificationService.UnSubscriptNotification();
        }

        public async void GoBack()
        {
            myWebview.RefreshCanGoBackForward();
            if (myWebview.CanGoBack)
            {
                myWebview.GoBack();
            }
            else
            {
                await Navigation.PopAsync(true);
            }
        }
    }
}