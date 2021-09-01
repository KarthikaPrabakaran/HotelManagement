using CustomerData.Model.Dto;
using CustomerData.Model.Enum;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;

namespace CustomerData
{
    public class ServiceApi
    {
        string baseURL = ConfigurationManager.AppSettings["apiBaseURL"];
        readonly string authDeniedMsg = "Authorization has been denied for this request";
        public static readonly string UserNotFoundError = "User name is not available!";
        public static readonly string ObjectIdNotFoundError = "Object Id is not available!";
        public static readonly string TokenNotFoundError = "Token is not available!";

        string secretKey = ConfigurationManager.AppSettings["SecretKey"];


        [WebMethod]
        public string HttpClientHandlerWithOutToken(RequestType RequestType, string Url, string postObject)
        {

            string stringData;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/text");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                HttpResponseMessage response = new HttpResponseMessage();
                var contentData = new StringContent(postObject, System.Text.Encoding.UTF8, "application/json");
                response = client.PostAsync(baseURL + Url, contentData).Result;
                stringData = response.Content.ReadAsStringAsync().Result;
            }
            return stringData;
        }


        [WebMethod]
        public string GetToken(string username, string password)
        {
            try
            {
                string stringData;
                var accessToken = "";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL);
                    MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/text");
                    client.DefaultRequestHeaders.Accept.Add(contentType);
                    client.DefaultRequestHeaders.Add("UserName", username); 
                    client.DefaultRequestHeaders.Add("Password", password);
                    client.DefaultRequestHeaders.Add("Source", "1");
                    HttpResponseMessage response = new HttpResponseMessage();
                    var contentData = new StringContent("", System.Text.Encoding.UTF8, "text/plain");
                    response = client.PostAsync(baseURL + "api/Home/GetToken", contentData).Result;
                    stringData = response.Content.ReadAsStringAsync().Result;

                    if (stringData.Contains("access_token"))
                    {
                        dynamic details = JObject.Parse(stringData);
                        accessToken = details.access_token;
                        return accessToken;
                    }
                    else
                    {
                        dynamic returnResponse = JObject.Parse(stringData);
                        throw new Exception(returnResponse.Message.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public string HttpClientHandlerWithToken(RequestType RequestType, string Url, string postObject, HttpSessionStateBase Session)
        {
            var token = Session["Token"]?.ToString();
            var userName = Session["Email"]?.ToString();
            var password = Session["CustomerObjectId"]?.ToString();

            if (string.IsNullOrWhiteSpace(token))
            {
                if (string.IsNullOrWhiteSpace(userName))
                    throw new Exception(UserNotFoundError);
                if (string.IsNullOrWhiteSpace(password))
                    throw new Exception(ObjectIdNotFoundError);
                token = GetToken(userName, password);
            }
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception(TokenNotFoundError);

            string stringData;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                //MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/text");
                //client.DefaultRequestHeaders.Accept.Add(contentType);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                HttpResponseMessage response = new HttpResponseMessage();
                if (RequestType == RequestType.Get)
                {
                    response = client.GetAsync(baseURL + Url).Result;
                }
                else if (RequestType == RequestType.Post)
                {

                    var contentData = new StringContent(postObject, System.Text.Encoding.UTF8, "application/json");
                    response = client.PostAsync(baseURL + Url, contentData).Result;
                }
                else if (RequestType == RequestType.Put)
                {
                    var contentData = new StringContent(postObject, System.Text.Encoding.UTF8, "application/json");
                    response = client.PutAsync(baseURL + Url, contentData).Result;
                }
                stringData = response.Content.ReadAsStringAsync().Result;
                if (stringData.Contains(authDeniedMsg))
                {
                    token = GetToken(userName, password);
                    Session["Token"] = token;
                    return HttpClientHandlerWithToken(RequestType, Url, postObject, Session);
                }

            }
            return stringData;
        }

        public MyBookingResponse LoadBookingHistory(MyBookingRequest customerViewRequest, HttpSessionStateBase session)
        {
            try
            {
                var requestString = JsonConvert.SerializeObject(customerViewRequest);
                var responseString = HttpClientHandlerWithToken(RequestType.Post, "api/Room/BookingHistory", requestString, session);
                var response = JsonConvert.DeserializeObject<ResponseDto<MyBookingResponse>>(responseString);
                var responseData = response.Data;
                if (response.StatusCode != 200)
                    throw new Exception(response.Message);
                foreach (var item in responseData.Items)
                {
                    item.BookedDate = (item.BookedDate == "" || item.BookedDate == null) ? "" : Convert.ToDateTime(item.BookedDate).ToString("dd-MM-yyy", CultureInfo.InvariantCulture);
                }


                return response.Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public BookRoomResponse BookRoom(BookRoomRequest customerViewRequest, HttpSessionStateBase session)
        {
            try
            {
                var requestString = JsonConvert.SerializeObject(customerViewRequest);
                var responseString = HttpClientHandlerWithToken(RequestType.Post, "api/Room/BookRoom", requestString, session);
                var response = JsonConvert.DeserializeObject<ResponseDto<BookRoomResponse>>(responseString);
                var responseData = response.Data;
                if (response.StatusCode != 200)
                    throw new Exception(response.Message);
                return response.Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CreateUser(AddCustomerRequest userDto, HttpSessionStateBase session)
        {
            try
            {
                var requestString = JsonConvert.SerializeObject(userDto);
                var responseString = HttpClientHandlerWithOutToken(RequestType.Post, "api/Room/CreateUser", requestString);
                var response = JsonConvert.DeserializeObject<ResponseDto<int>>(responseString);
                var responseData = response.Data;
                if (response.StatusCode != 200)
                    throw new Exception(response.Message);


                return response.Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CustomerDetail GetCustomerDetails(ValidateCustomerRequest request, HttpSessionStateBase session)
        {
            try
            {
                var requestString = JsonConvert.SerializeObject(request);
                var responseString = HttpClientHandlerWithToken(RequestType.Post, "api/Room/GetCustomerDetails", requestString, session);
                var response = JsonConvert.DeserializeObject<ResponseDto<CustomerDetail>>(responseString);
                var responseData = response.Data;
                if (response.StatusCode != 200)
                    throw new Exception(response.Message);
                return response.Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
