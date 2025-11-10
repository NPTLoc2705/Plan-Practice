using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Payments
{
    public class Package
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int CoinAmount { get; set; }
        [Required]
        public int Price { get; set; }


        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
