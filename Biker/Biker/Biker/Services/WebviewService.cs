﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TheS.DevXP.XamForms.Controls;

namespace Biker.Services
{
    public class WebviewService
    {
        public static readonly string MCLocalStorageFolderName = "mcontent/zip-biker";

        public static string GetHtmlPathByName(string pageName) 
        {
            var dirPath = Environment.SpecialFolder.LocalApplicationData;
            var defaultDirPath = Environment.GetFolderPath(dirPath);
            var destinationFolder = Path.Combine(defaultDirPath, MCLocalStorageFolderName);
            var MContentBaseUrl = string.Format("file://{0}/", destinationFolder);
            return $"{MContentBaseUrl}index.html#/{pageName}";
        }

        public static string ConvertObjectToUrlParameters(object parameters)
        {
            if (parameters == null) return string.Empty;

            var s = JsonConvert.SerializeObject(parameters);
            var d = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(s);

            var result = new StringBuilder();
            foreach (var key in d.Keys)
            {
                var hasvalue = !string.IsNullOrEmpty(d[key]);
                if (hasvalue)
                {
                    result.Append($";{key}={d[key]}");
                }
            }
            return result.ToString();
        }

        //public void RegisterServices(XWebView myWebview) 
        //{
        //    myWebview.RegisterNativeFunction("NavigateToPage", NavigateToPage);
        //    myWebview.RegisterNativeFunction("GetBikerId", GetBikerId);
        //    myWebview.RegisterCallback("Goback", Goback);
        //    myWebview.RegisterCallback("PopToRoot", PopToRoot);
        //    myWebview.RegisterCallback("SetPageTitle", SetPageTitle);
        //    myWebview.RegisterCallback("ExecuteNotiIfExist", ExecuteNotiIfExist);
        //    myWebview.RegisterCallback("RemoveNotificationChannel", RemoveNotificationChannel);
        //    myWebview.RegisterCallback("OpenMapDirection", OpenMapDirection);
        //    myWebview.RegisterCallback("PhoneCall", PhoneCall);
        //    myWebview.RegisterCallback("UpdateSidemenuItem", UpdateSidemenuItem);
        //}
    }
}
