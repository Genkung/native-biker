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
                try
                {
                    await BikerService.SetBikerInfo("1");
                    await NotificationService.RegisterDevice();
                    var bikerIsWorking = await BikerService.BikerIsWorking();
                    NavigateToMasterDetail(bikerIsWorking);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("แจ้งเตือน", "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง", "ปิด");
                }
            };

            b2.Clicked += async (s, e) =>
            {
                try
                {
                    await BikerService.SetBikerInfo("2");
                    await NotificationService.RegisterDevice();
                    var bikerIsWorking = await BikerService.BikerIsWorking();
                    NavigateToMasterDetail(bikerIsWorking);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("แจ้งเตือน", "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง", "ปิด");
                }
            };

            b3.Clicked += async (s, e) =>
            {
                try
                {
                    await BikerService.SetBikerInfo("3");
                    await NotificationService.RegisterDevice();
                    var bikerIsWorking = await BikerService.BikerIsWorking();
                    NavigateToMasterDetail(bikerIsWorking);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("แจ้งเตือน", "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง", "ปิด");
                }
            };
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