using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementDto.HotelRoom
{
    public class AvailableRoomListRequest
    {
        public string InputDate { get; set; }
        public string Sort { get; set; }
        public bool IsAscending { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
