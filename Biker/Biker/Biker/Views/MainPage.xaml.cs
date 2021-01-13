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
    public partial class MainPage : WebviewBase
    {
        public MainPage(string startPage, object parameters = null) : base(startPage, parameters)
        {
            InitializeComponent();
            AddXWebview(mWebview);
            InitWebview(startPage, parameters);
            RegisterWebviewBaseFunction(mWebview);
        }

        public override void InitWebview(string startPage, object parameters)
        {
            SidemenuService.UpdateSidemenuPage(SideMenuPageTitle.HomePage, startPage);

            mWebview.Accessors = new TheS.DevXP.XamForms.XWebViewAccessorCollection(
                LocalContentAccessor.GetAppData(WebviewService.MCLocalStorageFolderName));
            var htmlSource = WebviewService.GetHtmlPathByName(startPage);

            mWebview.NavigateOrRequesting += (s, e) =>
            {
                MessagingCenter.Send(this, MessagingChannel.HomeReady, string.Empty);
            };

            mWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
        }

        public override async Task<object[]> NavigateToPage(string param)
        {
            var paramObject = ConvertParameterFromWebView<NavigateToPageParameter>(param);
            if (paramObject != null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new WebViewPage(paramObject?.PageName, paramObject?.Params));
                });
            }

            return new object[] { true };
        }

        public override async void Goback(string param)
        {
        }

        public override async void PopToRoot(string param)
        {
        }

        public override async void SetRootPage(string param)
        {
            var paramObject = JsonConvert.DeserializeObject<NavigateToPageParameter>(param);
            Device.BeginInvokeOnMainThread(() =>
            {
                var htmlSource = WebviewService.GetHtmlPathByName(paramObject.PageName);
                mWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(paramObject.Params)}"; ;
            });
        }

        public void SideMenuChangePage(string page, object parameters)
        {
            var htmlSource = WebviewService.GetHtmlPathByName(page);
            mWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
        }
    }
}