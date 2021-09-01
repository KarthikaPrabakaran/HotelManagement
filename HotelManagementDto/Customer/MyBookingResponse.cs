using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementDto.Customer
{
    public class MyBookingResponse
    {
        public MyBookingResponse()
        {
        }
        public long CurrentPage { get; set; }
        public long TotalItems { get; set; }
        public long TotalPages { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public bool IsAscending { get; set; }
        public List<MyBooking> Items { get; set; }
    }

    public class MyBooking
    {
        public int RoomKey { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public string BookedDate { get; set; }
    }

}
