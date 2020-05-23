using Biker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biker.Services
{
    public class SidemenuService
    {
        private static List<SideMenuItem> menuList = new List<SideMenuItem>();

        public static void SetUpSideMenu()
        {
            menuList = new List<SideMenuItem>
            {
                new SideMenuItem{ Title = SideMenuPageTitle.HomePage, Page = "home" },
                new SideMenuItem{ Title = SideMenuPageTitle.Historypage, Page = "history-main" },
                new SideMenuItem{ Title = SideMenuPageTitle.ProfilePage, Page = "profile-main" }
            };
        }

        public static void UpdateSidemenuPage(string title, string page, object param = null)
        {
            menuList.FirstOrDefault(it => it.Title == title).Page = page;
            if (param != null)
            {
                menuList.FirstOrDefault(it => it.Title == title).Params = param;
            }
        }

        public static List<SideMenuItem> GetSidemenuItem()
        {
            return menuList;
        }
    }
}
