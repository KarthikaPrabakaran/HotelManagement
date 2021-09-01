using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using HotelManagement.IService;

namespace HotelManagement.Api.Controllers
{
    public class HomeController : ApiController
    {
        private HotelInfoIService HotelService;
        public HomeController(HotelInfoIService serviceRequest)
        {
            HotelService = serviceRequest;
        }

        string secretKey = ConfigurationManager.AppSettings["SecretKey"];
        string audience = ConfigurationManager.AppSettings["audience"];
        string issuer = ConfigurationManager.AppSettings["issuer"];


        [HttpPost]
        public object GetToken()
        {
            try
            {
                var userName = HttpContext.Current.Request.Headers.Get("UserName");
                var password = HttpContext.Current.Request.Headers.Get("Password");
                var source = HttpContext.Current.Request.Headers.Get("Source");
                if (source == "1")
                {
                    var response = HotelService.CheckLoggedInCUstomer(userName, password);
                    if(!response)
                    {
                        return new { StatusCode = 400, Message = "New Customer" };
                    }
                }
                else if(source == "2")
                {
                    var response = HotelService.CheckLoggedInOwner(userName, password);
                    if (!response)
                    {
                        return new { StatusCode = 400, Message = "Authorization has been denied for this request" };
                    }
                }
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var permClaims = new List<Claim>();
                //permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                permClaims.Add(new Claim(ClaimTypes.Role, userName));
                permClaims.Add(new Claim("password", password));
                permClaims.Add(new Claim("source", source));
                
                var token = new JwtSecurityToken(issuer,
                                audience,
                                permClaims,
                                expires: DateTime.Now.AddMonths(1),
                                signingCredentials: credentials);
                var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
                return new { access_token = jwt_token, token_type = "bearer", expires_in = DateTime.Now.AddDays(1), StatusCode = 200 };
            }
            catch(Exception e)
            { 
                return new { StatusCode = 400, Message = "Authorization has been denied for this request" };
            }
        }
    }
}
