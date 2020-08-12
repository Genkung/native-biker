using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Biker.Services
{
    public class CurrentWorkServive
    {
        static SecureStorageService secureStorage = new SecureStorageService();
        static string WORKID_KEY = "CurrentWork";

        public static async Task<string> GetCurrentWork()
        {
            var hasCurrentWork = await secureStorage.Contains(WORKID_KEY);
            if (hasCurrentWork)
            {
                var result = await secureStorage.Retrieve(WORKID_KEY);
                return result;
            }
            else
            {
                return "";
            }
        }

        public static async Task AddCurrentWork(string workId)
        {
            await secureStorage.Store(WORKID_KEY, workId);
        }

        public static void ClearCurrntWork()
        {
            secureStorage.Delete(WORKID_KEY);
        }
    }
}
