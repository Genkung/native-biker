using Biker.Models;
using Biker.Services;
using Biker.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheS.DevXP.XamForms.Controls;
using Xamarin.Forms;

namespace Biker
{
    public abstract class WebviewBase : ContentPage
    {
        public XWebView xWebview;
        public WebviewBase(string pageName, object parameter)
        {
        }

        public void AddXWebview(XWebView xWebview) 
        {
            this.xWebview = xWebview;
        }

        public abstract void InitWebview(string pageName, object parameter);

        public void RegisterWebviewBaseFunction(XWebView xWebview)
        {
            xWebview.RegisterNativeFunction("NavigateToPage", NavigateToPage);
            xWebview.RegisterNativeFunction("GetBikerId", GetBikerId);
            xWebview.RegisterCallback("Goback", Goback);
            xWebview.RegisterCallback("PopToRoot", PopToRoot);
            xWebview.RegisterCallback("SetRootPage", SetRootPage);
            xWebview.RegisterCallback("SetPageTitle", SetPageTitle);
            xWebview.RegisterCallback("ExecuteNotiIfExist", ExecuteNotiIfExist);
            xWebview.RegisterCallback("RemoveNotificationChannel", RemoveNotificationChannel);
            xWebview.RegisterCallback("OpenMapDirection", OpenMapDirection);
            xWebview.RegisterCallback("PhoneCall", PhoneCall);
            xWebview.RegisterCallback("UpdateSidemenuItem", UpdateSidemenuItem);
        }

        public abstract Task<object[]> NavigateToPage(string param);

        public abstract void Goback(string param);

        public abstract void PopToRoot(string param);

        public abstract void SetRootPage(string param);

        public async Task<object[]> GetBikerId(string param)
        {
            var biker = BikerService.GetBikerInfo();
            return new object[] { biker._id };
        }

        public async void SetPageTitle(string param)
        {
            var titleObj = ConvertParameterFromWebView<SetTitleParam>(param);
            Device.BeginInvokeOnMainThread(() =>
            {
                Title = titleObj?.Title;
            });
        }

        public async void ExecuteNotiIfExist(string param)
        {
            var notiChannelObj = ConvertParameterFromWebView<NotificationParameter>(param);
            NotificationService.ExecuteNotificationIfExist(notiChannelObj?.NotiChannel);
        }

        public async void RemoveNotificationChannel(string param)
        {
            var notiChannelObj = ConvertParameterFromWebView<NotificationParameter>(param);
            NotificationService.RemoveNotificationStack(notiChannelObj?.NotiChannel);
        }

        public async void OpenMapDirection(string directionParam)
        {
            var latLon = ConvertParameterFromWebView<OpenDirectionParam>(directionParam);
            if (latLon != null)
            {
                await GoogleMapService.OpenMapDirection(latLon.Latitude, latLon.Longitude);
            }
        }

        public async void PhoneCall(string directionParam) 
        {
            var latLon = ConvertParameterFromWebView<OpenDirectionParam>(directionParam);
            if (latLon != null)
            {
                await GoogleMapService.OpenMapDirection(latLon.Latitude, latLon.Longitude);
            }
        }

        public async void UpdateSidemenuItem(string param)
        {
            var sidemenu = ConvertParameterFromWebView<SideMenuItem>(param);
            SidemenuService.UpdateSidemenuPage(sidemenu?.Title, sidemenu?.Page, sidemenu?.Params);
        }

        public T ConvertParameterFromWebView<T>(string parameter)
        {
            try
            {
                var param = JsonConvert.DeserializeObject<T>(parameter);
                return param;
            }
            catch (Exception e)
            {
                return default;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            xWebview.Focus();

            NotificationService.SubscriptNotification((sender, obj) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await xWebview?.EvaluateJavaScriptAsync($"onSendNotification('{obj.NotiChannel}',{obj.Params});");
                });
            });

            xWebview.EvaluateJavaScriptAsync("refreshOnGoBack();");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NotificationService.UnSubscriptNotification();
        }
    }
}
