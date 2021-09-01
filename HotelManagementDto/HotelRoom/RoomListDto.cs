using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementDto.HotelRoom
{
    public class RoomListDto
    {
        public RoomListDto()
        {
        }
        public long CurrentPage { get; set; }
        public long TotalItems { get; set; }
        public long TotalPages { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public bool IsAscending { get; set; }
        public List<RoomDetail> Items { get; set; }
    }


    public class RoomDetail
    {
        public int RoomKey { get; set; }
        public string Type { get; set; }
        public string RoomNo { get; set; }
        public string Availability { get; set; }
    }

    public class RoomInfo
    {
        public int RoomKey { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }          
        public string Availability { get; set; }
    }
}
