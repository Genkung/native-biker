using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Biker.Services
{
    public class HttpClientService
    {
        private static readonly HttpClient client = new HttpClient();

        static HttpClientService() { }

        public static void SetClientBearer(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static async Task<T> Get<T>(string url)
        {
            var stringTask = await client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<T>(stringTask);
            return result;
        }

        public static async Task<System.Text.Json.JsonElement> GetJsonElement(string url)
        {
            var stringTask = await client.GetStringAsync(url);
            var result = ConvertFromText(stringTask);
            return result;
        }

        public static async Task<System.Text.Json.JsonElement> PostJsonElement(string url, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, data);
            var result = await response.Content.ReadAsStringAsync();
            return ConvertFromText(result);
        }

        public static async Task<System.Text.Json.JsonElement> PutJsonElement(string url, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, data);
            var result = await response.Content.ReadAsStringAsync();
            return ConvertFromText(result);
        }

        public static async Task Post(string url, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync(url, data);
        }

        public static async Task Put(string url, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(url, data);
        }

        public static System.Text.Json.JsonElement ConvertFromText(string txt)
        {
            try
            {
                var value = string.IsNullOrWhiteSpace(txt) ? "null" : txt;
                return System.Text.Json.JsonDocument.Parse(value).RootElement;
            }
            catch (Exception)
            {
                return System.Text.Json.JsonDocument.Parse("{}").RootElement;
            }
        }
    }
}
