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
    public partial class MainPage : ContentPage
    {
        public MainPage(string startPage, object parameters = null)
        {
            InitializeComponent();
            SetPage(startPage, parameters);
        }

        private async void SetPage(string startPage, object parameters)
        {
            SidemenuService.UpdateSidemenuPage(SideMenuPageTitle.HomePage, startPage);

            myWebview.Accessors = new TheS.DevXP.XamForms.XWebViewAccessorCollection(
                LocalContentAccessor.GetAppData(WebviewService.MCLocalStorageFolderName));
            var htmlSource = WebviewService.GetHtmlPathByName(startPage);

            myWebview.NavigateOrRequesting += (s, e) =>
            {
                MessagingCenter.Send(this, MessagingChannel.HomeReady, string.Empty);
            };

            RegisterWebviewBaseFunction();

            myWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}"; ;
        }

        private void RegisterWebviewBaseFunction()
        {
            myWebview.RegisterNativeFunction("NavigateToPage", NavigateToPage);
            myWebview.RegisterNativeFunction("GetBikerId", GetBikerId);
            myWebview.RegisterCallback("Goback", Goback);
            myWebview.RegisterCallback("PopToRoot", PopToRoot);
            myWebview.RegisterCallback("SetRootPage", SetRootPage);
            myWebview.RegisterCallback("SetPageTitle", SetPageTitle);
            myWebview.RegisterCallback("ExecuteNotiIfExist", ExecuteNotiIfExist);
            myWebview.RegisterCallback("RemoveNotificationChannel", RemoveNotificationChannel);
            myWebview.RegisterCallback("OpenMapDirection", OpenMapDirection);
            myWebview.RegisterCallback("PhoneCall", PhoneCall);
            myWebview.RegisterCallback("UpdateSidemenuItem", UpdateSidemenuItem);
        }

        private async Task<object[]> NavigateToPage(string param)
        {
            var paramObject = JsonConvert.DeserializeObject<NavigateToPageParameter>(param);
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new WebViewPage(paramObject.PageName, paramObject.Params));
            });
            return new object[] { true };
        }

        private async Task<object[]> GetBikerId(string param)
        {
            var biker = BikerService.GetBikerInfo();
            return new object[] { biker._id };
        }

        private async void Goback(string param)
        {
        }

        private async void PopToRoot(string param)
        {
        }

        private async void SetRootPage(string param)
        {
            var paramObject = JsonConvert.DeserializeObject<NavigateToPageParameter>(param);
            Device.BeginInvokeOnMainThread(() =>
            {
                var htmlSource = WebviewService.GetHtmlPathByName(paramObject.PageName);
                myWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(paramObject.Params)}"; ;
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

        public void SideMenuChangePage(string page, object parameters)
        {
            var htmlSource = WebviewService.GetHtmlPathByName(page);
            myWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
        }
    }
}