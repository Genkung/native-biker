using Biker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biker.Services
{
    public class BikerService
    {
        private static BikerModel bikerInfo;

        public static async void SetBikerInfo(string userid) 
        {
            var biker = await HttpClientService.Get<BikerModel>($"https://delivery-3rd-api.azurewebsites.net/api/Biker/GetBikerInfo/{userid}");
            bikerInfo = biker;
        }

        public static BikerModel GetBikerInfo() 
        {
            return bikerInfo;
        }
    }
}
