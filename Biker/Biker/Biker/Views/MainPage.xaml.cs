using Biker.Models;
using Biker.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var htmlSource = WebviewService.GetHtmlPathByName("master");

            myWebview.NavigateOrRequesting += (s, e) =>
            {
                MessagingCenter.Send(this, "HomeReady", string.Empty);
            };

            myWebview.RegisterNativeFunction("NavigateToPage", async param =>
            {
                var paramObject = JsonConvert.DeserializeObject<NavigateToPageParameter>(param);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new WebViewPage(paramObject.PageName, paramObject.Params));
                });
                return await Task.FromResult(new object[] { true });
            });

            myWebview.RegisterCallback("SetPageTitle", title =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Title = title;
                });
            });

            myWebview.Source = htmlSource;
        }

        public void ChangePage(string page,object parameters)
        {
            var htmlSource = WebviewService.GetHtmlPathByName(page);
            myWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
        }
    }
}