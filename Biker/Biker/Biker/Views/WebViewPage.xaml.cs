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

            myWebview.RegisterNativeFunction("NavigateToPage", async page =>
            {
                return await Task.FromResult(new object[] { false });
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

            myWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
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