using Biker.Models;
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
            myWebview.RegisterCallback("SetPageTitle", SetPageTitle);
            myWebview.RegisterCallback("ExecuteNotiIfExist", ExecuteNotiIfExist);
            myWebview.RegisterCallback("RemoveNotificationChannel", RemoveNotificationChannel);
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            myWebview.Focus();

            NotificationService.SubscriptNotification((sender, obj) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await myWebview?.EvaluateJavaScriptAsync($"onSendNotification('{obj.NotiKey}',{obj.Params});");
                });
            });

            myWebview.EvaluateJavaScriptAsync("refreshOnGoBack();");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NotificationService.UnSubscriptNotification();
        }
    }
}