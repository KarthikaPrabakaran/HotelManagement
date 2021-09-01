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
                   // ValidateIssuer = true,
                    //ValidateAudience = true,
                    //ValidateIssuerSigningKey = true,
                    //ValidIssuer = "http://mysite.com", //some string, normally web url,
                    //ValidAudience = "http://mysite.com",
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_12345"))
                }
            };
            app.UseJwtBearerAuthentication(options1);
            var config = new HttpConfiguration();
            //WebApiConfig.Register(config);
            //app.UseWebApi(config);
            WebApiConfig.Register(config);
            app.UseCors(CorsOptions.AllowAll);
            //app.UseWebApi(config);
        }
    }
}