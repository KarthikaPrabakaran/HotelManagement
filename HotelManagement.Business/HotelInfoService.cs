using HotelManagement.IRepository;
using HotelManagement.IService;
using HotelManagementDto.Customer;
using HotelManagementDto.HotelRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Business
{
    public class HotelInfoService : HotelInfoIService
    {
        HotelInfoIRepository HotelRepository;
        public HotelInfoService(HotelInfoIRepository paymentRepository)
        {
            HotelRepository = paymentRepository;
        }

        public string AddRoom(RoomDetailsRequest roomDetailsRequest)
        {
            return HotelRepository.AddRoom(roomDetailsRequest);
        }
        public RoomListDto GetAvailableRoomForOwner(AvailableRoomListRequest requestAvailableRoom)
        {
            return HotelRepository.GetAvailableRoomForOwner(requestAvailableRoom);
        }
        public BookedRoomDetail BookRoom(BookRoomRequest requestAvailableRoom)
        {
            return HotelRepository.BookRoom(requestAvailableRoom);
        }
        public MyBookingResponse GetBookedRoomHistory(MyBookingsRequest myBookingRequest)
        {
            return HotelRepository.GetBookedRoomHistory(myBookingRequest);
        }
        public int CreateUser(AddCustomerRequest userDto)
        {
            return HotelRepository.CreateUser(userDto);
        }
        public bool ValidateHotelOwner(ValidateOwnerRequest validateOwnerRequest)
        {
            return HotelRepository.ValidateHotelOwner(validateOwnerRequest);
        }

        public CustomerDetail GetCustomerDetails(ValidateCustomerRequest request)
        {
            return HotelRepository.GetCustomerDetails(request);
        }

        public bool CheckLoggedInCUstomer(string userName, string password)
        {
            return HotelRepository.CheckLoggedInCUstomer(userName,password);
        }
        public bool CheckLoggedInOwner(string userName, string password)
        {
            return HotelRepository.CheckLoggedInOwner(userName,password);
        }
    }
}
