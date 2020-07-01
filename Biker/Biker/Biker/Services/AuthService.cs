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
        private static AppToken appToken = new AppToken { AccessToken = new SecurityToken(), RefreshToken = new SecurityToken() };

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

        public static bool HasExpired => DateTime.UtcNow >= appToken?.AccessToken?.ExpiredDate.ToUniversalTime();

        public static async Task<bool> Login()
        {
            try
            {
                var authUrl = new Uri($"https://mana-auth-dev.azurewebsites.net/mobileauth/GoogleX/googlexauth");
                var callbackUrl = new Uri("googlexauth://");

                var result = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);

                var AuthToken = result?.AccessToken ?? result?.IdToken;

                appToken.AccessToken.Token = result?.IdToken;

                appToken.AccessToken.ExpiredDate = getTokenExpiredDate(appToken.AccessToken.Token);

                HttpClientService.SetClientBearerIfExistOrUnExpire(appToken.AccessToken.Token);

                return true;
            }
            catch (Exception e)
            {
                await PageService.DisplayAlert("แจ้งเตือน", "ไม่สามารถเข้าสู่ระบบได้ กรุณาลองใหม่อีกครั้ง", "ปิด");
                return false;
            }
        }

        public static async Task Logout()
        {
            try
            {
                var logoutUrl = $"https://mana-auth-dev.azurewebsites.net/mobileauth/signout";
                await HttpClientService.Logout();
            }
            catch (Exception e)
            {
            }
        }
    }
}
