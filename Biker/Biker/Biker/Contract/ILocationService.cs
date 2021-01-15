using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Biker.Contract
{
    public interface ILocationService
    {
        Task<bool> CanUseLocationService();
        Task<bool> RequestLocationService();
        Task<bool> RequestLocationPermission();
        Task<Location> GetCurrentLocation(bool fresh = false);
    }
}
