using LonerApp.Features.Author.Login.Models;
using System.IdentityModel.Tokens.Jwt;

namespace LonerApp.Helpers
{
    public static class JWTHelper
    {
        public static async Task<string> GetValidAccessToken()
        {
            var token = UserSetting.Get(StorageKey.AccessToken);
            if (string.IsNullOrEmpty(token))
                return token;

            if (IsTokenExpired(token))
            {
                var refreshToken = UserSetting.Get(StorageKey.RefreshToken);
                token = await RefreshTokenAsync(refreshToken);
                UserSetting.Set(StorageKey.AccessToken, token);
            }

            return token;
        }

        private static bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(-5);
            }
            catch
            {
                return true;
            }
        }

        private static async Task<string> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var response = await ServiceHelper.GetService<IApiService>().RefreshTokenAsync<RefreshTokenResponse>(refreshToken);
                UserSetting.Set(StorageKey.AccessToken, response?.AccessToken ?? "");
                UserSetting.Set(StorageKey.RefreshToken, response?.RefreshToken ?? "");

                return response?.AccessToken ?? "";
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to refresh token: {ex.Message}");
                return "";
            }
        }
    }
}