using Biker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Biker.Services
{
    public class BikerService
    {
        private static BikerModel bikerInfo;

        public static async Task SetBikerInfo(string userid) 
        {
            var biker = await HttpClientService.Get<BikerModel>($"https://delivery-3rd-api.azurewebsites.net/api/Biker/GetBikerInfo/{userid}");
            bikerInfo = biker;
        }

        public static async Task<bool> BikerIsWorking() 
        {
            var bikerWork = await HttpClientService.Get<object>($"https://delivery-3rd-api.azurewebsites.net/api/Biker/GetUnfinishedOrder/{bikerInfo?._id}");
            return bikerWork != null;
        } 

        public static BikerModel GetBikerInfo() 
        {
            return bikerInfo;
        }
    }
}
