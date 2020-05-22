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
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            myWebview.Accessors = new TheS.DevXP.XamForms.XWebViewAccessorCollection(
                LocalContentAccessor.GetAppData(WebviewService.MCLocalStorageFolderName));
            var htmlSource = WebviewService.GetHtmlPathByName("home");

            myWebview.NavigateOrRequesting += (s, e) =>
            {
                MessagingCenter.Send(this, MessagingChannel.HomeReady, string.Empty);
            };

            myWebview.RegisterNativeFunction("NavigateToPage", async param =>
            {
                var paramObject = JsonConvert.DeserializeObject<NavigateToPageParameter>(param);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new WebViewPage(paramObject.PageName, paramObject.Params));
                });
                return new object[] { true };
            });

            myWebview.RegisterNativeFunction("GetBikerId", async param =>
            {
                var biker = BikerService.GetBikerInfo();
                return new object[] { biker._id };
            });

            myWebview.RegisterCallback("SetPageTitle", title =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Title = title;
                });
            });

            myWebview.RegisterCallback("ExecuteNotiIfExist", notiChannel =>
            {
                NotificationService.ExecuteNotificationIfExist(notiChannel);
            });

            myWebview.RegisterCallback("RemoveNotificationChannel", notiChannel =>
            {
                NotificationService.RemoveNotificationStack(notiChannel);
            });

            myWebview.Source = htmlSource;
        }

        public void ChangePage(string page, object parameters)
        {
            var htmlSource = WebviewService.GetHtmlPathByName(page);
            myWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            myWebview.Focus();
            myWebview.EvaluateJavaScriptAsync("refreshOnGoBack();");

            NotificationService.SubscriptNotification((sender, obj) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await myWebview?.EvaluateJavaScriptAsync($"onSendNotification('{obj.NotiKey}',{obj.Params});");
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NotificationService.UnSubscriptNotification();
        }
    }
}