using Biker.Models;
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
    public partial class LogingQRAndLinkPage : ContentPage
    {
        public LogingQRAndLinkPage()
        {
            InitializeComponent();

            b1.Clicked += async (s, e) =>
            {
                await BikerService.SetBikerInfo("1");
                await NotificationService.RegisterDevice();
                NavigateToMasterDetail();
            };

            b2.Clicked += async (s, e) =>
            {
                await BikerService.SetBikerInfo("2");
                await NotificationService.RegisterDevice();
                NavigateToMasterDetail();
            };

            b3.Clicked += async (s, e) =>
            {
                await BikerService.SetBikerInfo("3");
                await NotificationService.RegisterDevice();
                NavigateToMasterDetail();
            };
        }

        private void NavigateToMasterDetail() 
        {
            SidemenuService.SetUpSideMenu();

            var homePage = new NavigationPage(new MainPage());
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