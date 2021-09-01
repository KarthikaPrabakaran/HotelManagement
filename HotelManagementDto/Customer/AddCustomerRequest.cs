using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementDto.Customer
{
    public class AddCustomerRequest
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string CustomerObjectId { get; set; }
    }
}
