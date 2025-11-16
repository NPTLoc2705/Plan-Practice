using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.paymentDto
{
    public class CreatePackageDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Coin amount must be greater than 0")]
        public int CoinAmount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int Price { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdatePackageDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Coin amount must be greater than 0")]
        public int? CoinAmount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int? Price { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }

    public class PackageDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CoinAmount { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int TotalPurchases { get; set; }
        public int TotalRevenue { get; set; }
    }

    public class PackageListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CoinAmount { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
