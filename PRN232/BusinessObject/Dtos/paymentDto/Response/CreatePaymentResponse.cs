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
    public class PaymentTransactionDto
    {
        public long OrderCode { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int CoinAmount { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string TransactionCode { get; set; }
    }

    public class PaymentRevenueDto
    {
        public int TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalCoins { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<RevenueByPackageDto> RevenueByPackage { get; set; }
        public List<DailyRevenueDto> DailyRevenue { get; set; }
    }

    public class RevenueByPackageDto
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalCoins { get; set; }
    }

    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public int Revenue { get; set; }
        public int TransactionCount { get; set; }
    }
}
