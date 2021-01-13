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
    public partial class WebViewPage : WebviewBase
    {
        public WebViewPage(string pageName, object parameters) : base(pageName, parameters)
        {
            InitializeComponent();
            AddXWebview(wWebview);
            InitWebview(pageName, parameters);
            RegisterWebviewBaseFunction(wWebview);
        }

        public override void InitWebview(string page, object parameters)
        {
            wWebview.Accessors = new TheS.DevXP.XamForms.XWebViewAccessorCollection(
                LocalContentAccessor.GetAppData(WebviewService.MCLocalStorageFolderName));

            var htmlSource = WebviewService.GetHtmlPathByName(page);

            wWebview.Source = $"{htmlSource}{WebviewService.ConvertObjectToUrlParameters(parameters)}";
        }

        public override async Task<object[]> NavigateToPage(string param)
        {
            return new object[] { false };
        }

        public override async void Goback(string param)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                GoBack();
            });
        }

        public override async void PopToRoot(string param)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopAsync(true);
            });
        }

        public override async void SetRootPage(string param)
        {
            var paramObject = JsonConvert.DeserializeObject<NavigateToPageParameter>(param);
            Device.BeginInvokeOnMainThread(() =>
            {
                ((MasterDetailPage)Application.Current.MainPage).Detail = new NavigationPage(new MainPage(paramObject.PageName));
            });
        }

        public async void GoBack()
        {
            wWebview.RefreshCanGoBackForward();
            if (wWebview.CanGoBack)
            {
                wWebview.GoBack();
            }
            else
            {
                await Navigation.PopAsync(true);
            }
        }
    }
}