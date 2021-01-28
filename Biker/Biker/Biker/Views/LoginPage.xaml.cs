using Biker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Biker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            loginBtn.Clicked += async (s, e) =>
            {
                SetCanLogin(false);

                var isLogin = await AuthService.IDP4Login();

                if (isLogin)
                {
                    try
                    {
                        await NotificationService.RegisterDevice();

                        var bikerIsWorking = await BikerService.BikerIsWorking();

                        NavigateToMasterDetail(bikerIsWorking);
                    }
                    catch (Exception ex)
                    {
                        PageService.DisplayAlert("แจ้งเตือน", "ไม่สามารถเข้าสู่ระบบได้ กรุณาลองใหม่อีกครั้ง", "ปิด");
                        SetCanLogin(true);
                    }
                }
                else
                {
                    SetCanLogin(true);
                }
            };
        }

        private void SetCanLogin(bool needLigin)
        {
            loginBtn.IsVisible = needLigin;
            loadingLogin.IsRunning = !needLigin;
            loadingLogin.IsVisible = !needLigin;
        }

        private void NavigateToMasterDetail(bool bikerIsWorking)
        {
            SidemenuService.SetUpSideMenu();

            var startPage = bikerIsWorking ? "order-stage" : "home";

            var homePage = new NavigationPage(new MainPage(startPage));
            var masterDetailPage = new MasterDetailPage
            {
                Detail = homePage,
                IsGestureEnabled = false
            };

            masterDetailPage.Master = new MenuPage();
            masterDetailPage.Master.IconImageSource = "hammenu";
            App.Current.MainPage = masterDetailPage;
        }
    }
}