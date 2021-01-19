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

        public void RegisterWebviewBaseFunction(XWebView xWebview)
        {
            xWebview.RegisterNativeFunction("NavigateToPage", NavigateToPage);
            xWebview.RegisterNativeFunction("GetBikerId", GetBikerId);
            xWebview.RegisterNativeFunction("GetCurrentLocation", GetCurrentLocation);
            xWebview.RegisterNativeFunction("CallApiGet", CallApiGet);
            xWebview.RegisterNativeFunction("CallApiPost", CallApiPost);
            xWebview.RegisterNativeFunction("CallApiPut", CallApiPut);
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

        public abstract void InitWebview(string pageName, object parameter);

        public abstract Task<object[]> NavigateToPage(string param);

        public abstract void Goback(string param);

        public abstract void PopToRoot(string param);

        public abstract void SetRootPage(string param);

        protected virtual async Task<object[]> CallApiGet(string urlparam)
        {
            var parsedParam = ConvertParameterFromWebView<CallApiGetParam>(urlparam);
            if (parsedParam != null)
            {
                var responseBody = await HttpClientService.GetJsonElement(parsedParam.Url);
                return new object[] { responseBody };
            }
            else
            {
                return new object[] { "{}" };
            }
        }

        protected virtual async Task<object[]> CallApiPost(string postparam)
        {
            var parsedParam = ConvertParameterFromWebView<CallApiPostParam>(postparam);
            if (parsedParam != null)
            {
                var responseBody = await HttpClientService.PostJsonElement(parsedParam.Url, parsedParam.Data);
                return new object[] { responseBody };
            }
            else
            {
                return new object[] { "{}" };
            }
        }

        protected virtual async Task<object[]> CallApiPut(string postparam)
        {
            var parsedParam = ConvertParameterFromWebView<CallApiPostParam>(postparam);
            if (parsedParam != null)
            {
                var responseBody = await HttpClientService.PutJsonElement(parsedParam.Url, parsedParam.Data);
                return new object[] { responseBody };
            }
            else
            {
                return new object[] { "{}" };
            }
        }

        public async Task<object[]> GetCurrentLocation(string param)
        {
            var location = await GPSService.GetCurrentLocation();
            return new object[] { location };
        }

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
                await GPSService.OpenMapDirection(latLon.Latitude, latLon.Longitude);
            }
            else { PageService.DisplayAlert("แจ้งเตือน", "ไม่สามารถเปิดแผนที่ได้ กรุณาลองใหม่อีกครัง", "ปิด"); }
        }

        public async void PhoneCall(string phoneParam)
        {
            var phome = ConvertParameterFromWebView<PhoneCallParameter>(phoneParam);
            if (phome != null)
            {
                PhoneService.Call(phome?.PhoneNumber);
            }
            else { PageService.DisplayAlert("แจ้งเตือน", "ขออภัย เกิดข้อผิดพลาด", "ปิด"); }
        }

        public async void UpdateSidemenuItem(string param)
        {
            var sidemenu = ConvertParameterFromWebView<SideMenuItem>(param);
            SidemenuService.UpdateSidemenuPage(sidemenu?.Title, sidemenu?.Page, sidemenu.Params);
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
