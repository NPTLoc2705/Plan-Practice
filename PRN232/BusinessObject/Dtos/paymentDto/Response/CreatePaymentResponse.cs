using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.paymentDto.Response
{
    public class CreatePaymentResponse
    {
        public string CheckoutUrl { get; set; }
        public long OrderCode { get; set; }
        public string PaymentLinkId { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
    }

    public class PaymentStatusResponse
    {
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string TransactionCode { get; set; }
    }
}
