using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementDto.Customer
{
    public class BookRoomRequest
    {
        public int CustomerKey { get; set; }
        public string RoomType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
