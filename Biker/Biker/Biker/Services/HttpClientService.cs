using Biker.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Biker.Services
{
    public class HttpClientService
    {
        private static readonly HttpClient client = new HttpClient();

        public static void SetClientBearer(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private static bool hasAuthorization => string.IsNullOrWhiteSpace(client.DefaultRequestHeaders.Authorization.ToString());

        //private static bool needLogin => hasAuthorization && AuthService.HasExpired;
        private static bool needLogin => AuthService.HasExpired;

        private static async Task LogoutIfNeeded()
        {
            //TODO: Logout
            PageService.SetRoot(new LoginPage());
        }

        public static async Task<T> Get<T>(string url)
        {
            if (needLogin)
            {
                await LogoutIfNeeded();
                throw new System.ArgumentException("session หมดอายุ กรุณา login ใหม่อีกครั้ง", "expired");
            }

            var stringTask = await client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<T>(stringTask);
            return result;
        }

        public static async Task Logout()
        {
            DependencyService.Get<IClearCookies>().Clear();
            var url = "https://oauth2.googleapis.com/revoke";
            var sendData = new List<KeyValuePair<string, string>>();
            sendData.Add(new KeyValuePair<string, string>("token", AuthService.GetAccessToken()));
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(sendData) };
            var res = await client.SendAsync(req);
        }

        public static async Task Post(string url, object body)
        {
            if (needLogin)
            {
                await LogoutIfNeeded();
                throw new System.ArgumentException("session หมดอายุ กรุณา login ใหม่อีกครั้ง", "expired");
            }

            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync(url, data);
        }

        public static async Task Put(string url, object body)
        {
            if (needLogin)
            {
                await LogoutIfNeeded();
                throw new System.ArgumentException("session หมดอายุ กรุณา login ใหม่อีกครั้ง", "expired");
            }

            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(url, data);
        }

        public static async void HandleHttpCatch(Exception e)
        {
            if (e is ArgumentException arg)
            {
                await PageService.DisplayAlert("แจ้งเตือน", arg.Message, "ปิด");
            }
            else
            {
                await PageService.DisplayError();
            }
        }
    }
}
