using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerData.Model.Dto
{
    public class CustomerDetail
    {
        public int CustomerKey { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string CustomerObjectId { get; set; }
    }
}
