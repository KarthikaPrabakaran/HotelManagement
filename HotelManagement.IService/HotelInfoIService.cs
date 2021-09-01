using HotelManagementDto.Customer;
using HotelManagementDto.HotelRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.IService
{
    public interface HotelInfoIService
    {
        string AddRoom(RoomDetailsRequest roomDetailsRequest);
        RoomListDto GetAvailableRoomForOwner(AvailableRoomListRequest requestAvailableRoom);
        BookedRoomDetail BookRoom(BookRoomRequest requestAvailableRoom);
        MyBookingResponse GetBookedRoomHistory(MyBookingsRequest myBookingRequest);
        int CreateUser(AddCustomerRequest userDto);
        bool ValidateHotelOwner(ValidateOwnerRequest validateOwnerRequest);
        CustomerDetail GetCustomerDetails(ValidateCustomerRequest request);
        bool CheckLoggedInCUstomer(string userName, string password);
        bool CheckLoggedInOwner(string userName, string password);
    }
}
