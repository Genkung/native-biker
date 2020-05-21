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

            GotoMain.Clicked += (s, e) =>
            {
                BikerService.SetBikerInfo("2");
                NavigateToMasterDetail();
            };
        }

        private void NavigateToMasterDetail() 
        {
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