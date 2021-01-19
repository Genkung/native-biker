using Biker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
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

            ids4loginBtn.Clicked += async (s, e) =>
            {
                SetCanLogin(false);

                var isLogin = await AuthService.IDP4Login();

                if (isLogin)
                {
                    try
                    {
                        await BikerService.SetBikerInfo("1");
                        await NotificationService.RegisterDevice();

                        var bikerIsWorking = await BikerService.BikerIsWorking();

                        NavigateToMasterDetail(bikerIsWorking);
                    }
                    catch (Exception ex)
                    {
                        HttpClientService.HandleHttpCatch(ex);
                        SetCanLogin(true);
                    }
                }
                else
                {
                    SetCanLogin(true);
                }
            };

            gloginBtn.Clicked += async (s, e) =>
            {
                SetCanLogin(false);

                var isLogin = await AuthService.GLogin();

                if (isLogin)
                {
                    try
                    {
                        await BikerService.SetBikerInfo("1");
                        await NotificationService.RegisterDevice();

                        var bikerIsWorking = await BikerService.BikerIsWorking();

                        NavigateToMasterDetail(bikerIsWorking);
                    }
                    catch (Exception ex)
                    {
                        HttpClientService.HandleHttpCatch(ex);
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
            login.IsVisible = needLigin;
            loadingLogin.IsRunning = !needLigin;
            loadingLogin.IsVisible = !needLigin;
        }

        private void NavigateToMasterDetail(bool bikerIsWorking)
        {
            SidemenuService.SetUpSideMenu();

            var homePage = new NavigationPage(new MainPage(bikerIsWorking));
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