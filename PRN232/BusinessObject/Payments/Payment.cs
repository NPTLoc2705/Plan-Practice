using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Payments
{
    public class Payment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public long OrderCode { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(100)]
        public string? Status { get; set; } // PENDING, PAID, CANCELLED

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }

        [StringLength(500)]
        public string? PaymentLinkId { get; set; }

        [StringLength(50)]
        public string? TransactionCode { get; set; }

        // Navigation the user 
        public User? User { get; set; }

        // Navigation The Package
        public int PackageId { get; set; }
        public virtual Package Package { get; set; }
    }
}
