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

        public static string RefId;

        public static bool HasExpired => DateTime.UtcNow >= appToken?.AccessToken?.ExpiredDate.ToUniversalTime();

        private static DateTime getRefrstokenExpiredDate(WebAuthenticatorResult authResult)
        {
            if (authResult == null) return DateTime.UtcNow;

            long expNum;

            if (authResult.Properties.TryGetValue("expires", out string expstring))
            {
                expNum = Convert.ToInt64(expstring);
            }
            else return DateTime.UtcNow;

            var result = DateTimeOffset.FromUnixTimeSeconds(expNum).UtcDateTime;
            return result;
        }

        private static DateTime getTokenExpiredDate(string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            if (!jwtTokenHandler.CanReadToken(token)) return DateTime.UtcNow;
            var jwtData = jwtTokenHandler.ReadJwtToken(token);
            return jwtData.ValidTo.ToUniversalTime();
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
                appToken.RefreshToken.ExpiredDate = getRefrstokenExpiredDate(result);

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

        public static async Task RefreshToken()
        {
            var refreshToken = await HttpClientService.Get<RefreshTokenResponse>($"https://pilotdeli-mvc.azurewebsites.net/mobileauth/refreshtoken/{appToken.RefreshToken.Token}");
            appToken.AccessToken.Token = refreshToken?.AccessToken;
            appToken.RefreshToken.Token = refreshToken?.RefreshToken;
        }

        public static async Task Logout()
        {
            try
            {
                //await Browser.OpenAsync("https://pilotdeli-mvc.azurewebsites.net/mobileauth/logout2", BrowserLaunchMode.SystemPreferred);
#if __ANDROID__
                var authUrl = new Uri($"https://pilotdeli-mvc.azurewebsites.net/mobileauth/logout");
                var callbackUrl = new Uri(CALLBACK_URL);
                await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
#endif
                BikerService.ClearRider();
            }
            catch (Exception e)
            {
            }
        }
    }
}