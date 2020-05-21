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

            App.notiservice.SubscriptNotification((sender, param) =>
            {
                Device.BeginInvokeOnMainThread(async () => {
                    await myWebview.EvaluateJavaScriptAsync($"onSendNotification('{param.NotiKey}',{param.Params});");
                });
            });

            myWebview.RegisterNativeFunction("NavigateToPage", async param =>
            {
                var paramObject = JsonConvert.DeserializeObject<NavigateToPageParameter>(param);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new WebViewPage(paramObject.PageName, paramObject.Params));
                });
                return await Task.FromResult(new object[] { true });
            });

            myWebview.RegisterNativeFunction("GetBikerId", async param =>
            {
                var biker = BikerService.GetBikerInfo();
                return await Task.FromResult(new object[] { biker._id });
            });

            myWebview.RegisterCallback("SetPageTitle", title =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Title = title;
                });
            });

            myWebview.RegisterNativeFunction("GetParamsInNotificationStack", async notikey =>
            {
                return await Task.FromResult(new object[] { App.notiservice.GetParamsInNotificationStack(notikey) });
            });

            myWebview.Source = htmlSource;
        }

        public void ChangePage(string page,object parameters)
        {
            var htmlSource = WebviewService.GetHtmlPathByName(page);
            myWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            myWebview.Focus();
            myWebview.EvaluateJavaScriptAsync("refreshOnGoBack();");
        }
    }
}