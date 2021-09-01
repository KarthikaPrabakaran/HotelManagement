using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelManagement.Api;
using System.Web.Http;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(Startup))]

namespace HotelManagement.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            JwtBearerAuthenticationOptions options1 = new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters()
                {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                }
            };
            app.UseJwtBearerAuthentication(options1);
        }
    }
}
