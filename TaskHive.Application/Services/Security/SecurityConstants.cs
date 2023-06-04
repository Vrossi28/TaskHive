using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Application.Services.Security
{
    public static class SecurityConstants
    {
        public static string GeolocationUrl = "Geolocation:BaseUrl";
        public static string JwtSecret = "Jwt:SecretKey";
        public static string JwtIssuer = "Jwt:Issuer";
        public static string JwtAudience = "Jwt:Audience";
    }
}
