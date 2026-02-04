using Microsoft.AspNetCore.Http;

namespace WebApp.WebAppUtilities
{
    public static class CookieUtil
    {
        public static CookieOptions AccessToken(bool isDev)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDev,          
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            };
        }

        public static CookieOptions RefreshToken(bool isDev)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDev,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
        }

        public static CookieOptions Logout()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            };
        }
    }
}
