using HotelManagement.IService;
using HotelManagementDto;
using HotelManagementDto.Customer;
using HotelManagementDto.HotelRoom;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Http;


namespace HotelManagement.Api.Controllers
{
    public class RoomController : ApiController
    {
        private HotelInfoIService HotelService;
        public RoomController(HotelInfoIService serviceRequest)
        {
            HotelService = serviceRequest;
        }

        //[Authorize(Roles = "karthika95praba@gmail.com")]
        public ResponseDto<MyBookingResponse> BookingHistory(MyBookingsRequest request)
        {
            try
            {
                var productDetails = HotelService.GetBookedRoomHistory(request);

                return new ResponseDto<MyBookingResponse>()
                {
                    Data = productDetails
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<MyBookingResponse>()
                {
                    StatusCode = 500,
                    Message = e.Message
                };
            }
        }

        ///[Authorize]
        public ResponseDto<BookedRoomDetail> BookRoom(BookRoomRequest request)
        {
            try
            {
                var details = HotelService.BookRoom(request);

                return new ResponseDto<BookedRoomDetail>()
                {
                    Data = details
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<BookedRoomDetail>()
                {
                    StatusCode = 500,
                    Message = e.Message
                };
            }
        }

        //[Authorize]
        public ResponseDto<int> CreateUser(AddCustomerRequest request)
        {
            try
            {
                var details = HotelService.CreateUser(request);

                return new ResponseDto<int>()
                {
                    Data = details
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<int>()
                {
                    StatusCode = 500,
                    Message = e.Message
                };
            }
        }

        //[Authorize]
        public ResponseDto<string> AddRoom(RoomDetailsRequest request)
        {
            try
            {
                var details = HotelService.AddRoom(request);

                return new ResponseDto<string>()
                {
                    Data = details
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<string>()
                {
                    StatusCode = 500,
                    Message = e.Message
                };
            }
        }

        //[Authorize]
        [HttpPost]
        public ResponseDto<CustomerDetail> GetCustomerDetails(ValidateCustomerRequest request)
        {
            try
            {
                var details = HotelService.GetCustomerDetails(request);
                return new ResponseDto<CustomerDetail>()
                {
                    Data = details
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<CustomerDetail>()
                {
                    StatusCode = 500,
                    Message = e.Message
                };
            }
        }
        //[Authorize]
        [HttpPost]
        public ResponseDto<List<RoomDetail>> GetAvailableRoomForOwner(AvailableRoomListRequest request)
        {
            try
            {
                var details = HotelService.GetAvailableRoomForOwner(request);
                return new ResponseDto<List<RoomDetail>>()
                {
                    Data = details.Items
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<List<RoomDetail>>()
                {
                    StatusCode = 500,
                    Message = e.Message
                };
            }
        }
    }
}