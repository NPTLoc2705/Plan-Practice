using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.paymentDto.Request
{
    public class CreatePaymentRequest
    {
        // Optional - will be overridden by JWT token userId
     
        public int PackageId { get; set; }
        // Optional description for the payment
        public string? Description { get; set; }
    }
}
