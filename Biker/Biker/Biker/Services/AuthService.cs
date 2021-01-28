using Biker.Models;
using Biker.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Biker.Services
{
    public class AuthService
    {
        private static readonly string CALLBACK_URL = "mauth://";

        private static AppToken appToken = new AppToken { AccessToken = new SecurityToken(), RefreshToken = new SecurityToken() };

        public static bool HasAccessTokenExpired => DateTime.UtcNow >= appToken?.AccessToken?.ExpiredDate.ToUniversalTime();

        private static DateTime getTokenExpiredDate(string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            if (!jwtTokenHandler.CanReadToken(token)) return DateTime.UtcNow;
            var jwtData = jwtTokenHandler.ReadJwtToken(token);
            return jwtData.ValidTo.ToUniversalTime();
        }

        private static async Task<bool> NeedLoginAgain()
        {
            if (HasAccessTokenExpired)
            {
                return !await RefreshToken();
            }
            else return false;
        }

        public static string GetAccessToken()
        {
            return appToken.AccessToken.Token;
        }

        public static async Task<bool> IDP4Login()
        {
            try
            {
                var authUrl = new Uri($"https://pilotdeli-mvc.azurewebsites.net/mobileauth/login");
                var callbackUrl = new Uri(CALLBACK_URL);
                var result = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);

                appToken.AccessToken.Token = result?.AccessToken;
                appToken.RefreshToken.Token = result?.RefreshToken;

                appToken.AccessToken.ExpiredDate = getTokenExpiredDate(appToken.AccessToken.Token);

                var needLogout = await NeedLoginAgain();

                if (needLogout)
                {
                    await App.Current.MainPage.DisplayAlert("แจ้งเตือน", $"Session หมด อายุ", "ปิด");
                    await Logout();
                    return false;
                }

                var RefId = result.Properties.ContainsKey("ref_id") ? result.Properties["ref_id"] : "";

                if (!string.IsNullOrWhiteSpace(RefId))
                {
                    await BikerService.SetBikerInfo(RefId);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("แจ้งเตือน", $"คุณยังไม่ได้ยืนยัน Consent", "ปิด");
                    await Logout();
                    return false;
                }

                //TODO: SetClientBearer when server use real IDP 
                //HttpClientService.SetClientBearer(appToken.AccessToken.Token);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> RefreshToken()
        {
            var refreshToken = await HttpClientService.Get<RefreshTokenResponse>($"https://pilotdeli-mvc.azurewebsites.net/mobileauth/refreshtoken/{appToken.RefreshToken.Token}");
            if (string.IsNullOrWhiteSpace(refreshToken?.AccessToken) || string.IsNullOrWhiteSpace(refreshToken?.RefreshToken))
            {
                return false;
            }
            appToken.AccessToken.Token = refreshToken?.AccessToken;
            appToken.RefreshToken.Token = refreshToken?.RefreshToken;
            return true;
        }

        public static async Task Logout()
        {
            try
            {
                //await Browser.OpenAsync("https://pilotdeli-mvc.azurewebsites.net/mobileauth/logout2", BrowserLaunchMode.SystemPreferred);
                if (Device.RuntimePlatform == Device.Android)
                {
                    var authUrl = new Uri($"https://pilotdeli-mvc.azurewebsites.net/mobileauth/logout");
                    var callbackUrl = new Uri(CALLBACK_URL);
                    await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
                }

                BikerService.ClearRider();
            }
            catch (Exception e)
            {
            }
        }
    }
}