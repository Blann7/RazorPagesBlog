using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogPhone.Models.Auth
{
    public static class AuthOptions
    {
        public const string REGISTERED_KEY = "_4NVWENWysWKPBnygP3ngdZZQQ2H3VF7D5EMAmHyQsJ7c5RazaRKdEY5T97DN";


        public const string ISSUER = "BP Server";
        public const string AUDIENCE = "BP Client"; 
        private const string KEY = "uP3k+5Hu&sJG2!NwG+a9%%ZG$G?ZUX-E^W-Sw6Y!^^PV+C!r";
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
