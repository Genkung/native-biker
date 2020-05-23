﻿using Biker.Models;
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
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            var userProfile = BikerService.GetBikerInfo();

            userProfileImage.Source = userProfile.profileImage;
            userProfileName.Text = userProfile.Name;

            var menuList = SidemenuService.GetSidemenuItem();

            ListViewMenu.ItemsSource = menuList;
            ListViewMenu.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null) return;

                var page = ((SideMenuItem)e.SelectedItem).Page;
                var parameters = ((SideMenuItem)e.SelectedItem).Params;
                PageService.GetRootPage().SideMenuChangePage(page, parameters);
                ((MasterDetailPage)Application.Current.MainPage).IsPresented = false;
                ListViewMenu.SelectedItem = null;
            };
        }
    }
}