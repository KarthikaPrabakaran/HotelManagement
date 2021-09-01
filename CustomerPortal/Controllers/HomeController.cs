using DotNetOpenAuth.GoogleOAuth2;
using Microsoft.AspNet.Membership.OpenAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomerData.Model.Dto;
using CustomerData;
using System.Web.Services.Description;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;

namespace CustomerPortal.Controllers
{
    public class HomeController : Controller
    {
        ServiceApi serverApi = new ServiceApi();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult BookingHistory()
        {
            return View();
        }


        public ActionResult RedirectToGoogle()
        {
            string provider = "google";
            string returnUrl = "";
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }
        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OpenAuth.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            string ProviderName = OpenAuth.GetProviderNameFromCurrentRequest();

            if (ProviderName == null || ProviderName == "")
            {
                NameValueCollection nvs = Request.QueryString;
                if (nvs.Count > 0)
                {
                    if (nvs["state"] != null)
                    {
                        NameValueCollection provideritem = HttpUtility.ParseQueryString(nvs["state"]);
                        if (provideritem["__provider__"] != null)
                        {
                            ProviderName = provideritem["__provider__"];
                        }
                    }
                }
            }
            GoogleOAuth2Client.RewriteRequest();
            var redirectUrl = Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl });
            var authResult = OpenAuth.VerifyAuthentication(redirectUrl);

            if (!authResult.IsSuccessful)
            {

                return Redirect(Url.Action("Home", "Login"));
            }
            Session["UserName"] = authResult.UserName ?? "";
            Session["CustomerObjectId"] = authResult.ProviderUserId ?? "";
            ViewData["UserName"] = authResult.UserName ?? "";
            Session["Email"] = authResult.ExtraData["email"];
            try
            {
                var response = serverApi.GetToken((string)Session["Email"], (string)Session["CustomerObjectId"]);
                Session["Token"] = response;
                ValidateCustomerRequest validateCustomerRequest = new ValidateCustomerRequest();
                validateCustomerRequest.Email = authResult.ExtraData["email"];
                var customer = serverApi.GetCustomerDetails(validateCustomerRequest, Session);
                Session["CustomerKey"] = customer.CustomerKey;
            }
            catch (Exception ex)
            {
                if (ex.Message == "New Customer")
                {
                    AddCustomerRequest addCustomerRequest = new AddCustomerRequest();
                    addCustomerRequest.CustomerName = authResult.UserName;
                    addCustomerRequest.Email = authResult.ExtraData["email"];
                    addCustomerRequest.CustomerObjectId = authResult.ProviderUserId;
                    var customer = serverApi.CreateUser(addCustomerRequest, Session);
                    Session["CustomerKey"] = customer;
                    var response = serverApi.GetToken((string)Session["Email"], (string)Session["CustomerObjectId"]);
                    Session["Token"] = response;
                }
                else
                {
                    return Redirect(Url.Action("RedirectToGoogle", "Home"));

                }
            }
            return Redirect(Url.Action("Index", "Home"));
           
        }


        [HttpPost]
        public JsonResult BookingHistory(MyBookingRequest request)
        {
            try
            {  
                request.CustomerKey = Session["CustomerKey"] != null ? Int32.Parse(Session["CustomerKey"].ToString()) :0;             
                var productDetails = serverApi.LoadBookingHistory(request,Session);

                return Json(new ResponseDto<MyBookingResponse>()
                {
                    Data = productDetails
                });
            }
            catch (Exception e)
            {
                return Json(new ResponseDto<MyBookingResponse>()
                {
                    StatusCode = 500,
                    Message = e.Message
                });
            }
        } 
        
        [HttpPost]
        public JsonResult BookRoom(BookRoomRequest request)
        {
            try
            {
                request.CustomerKey = (int)Session["CustomerKey"];
                var details = serverApi.BookRoom(request,Session);

                return Json(new ResponseDto<BookRoomResponse>()
                {
                    Data = details
                });
            }
            catch (Exception e)
            {
                return Json(new ResponseDto<BookRoomResponse>()
                {
                    StatusCode = 500,
                    Message = e.Message
                });
            }
        }

      

    }
}