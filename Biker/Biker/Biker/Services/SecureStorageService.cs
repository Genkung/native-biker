using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using xames = Xamarin.Essentials;

namespace Biker.Services
{
    public class SecureStorageService
    {
        public async Task<bool> Contains(string key)
        {
            var data = await xames.SecureStorage.GetAsync(key);
            return data != null;
        }

        public async void Delete(string key)
        {
            if (await Contains(key))
            {
                xames.SecureStorage.Remove(key);
            }
        }

        public async Task<string> Retrieve(string key)
        {
            return await xames.SecureStorage.GetAsync(key);
        }

        public async Task Store(string key, string data)
        {
            await xames.SecureStorage.SetAsync(key, data);
        }
    }
}
