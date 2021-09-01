using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementDto
{
    public class ResponseDto<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResponseDto()
        {
            StatusCode = 200;
            Message = "SUCCESS";
        }
    }
}
