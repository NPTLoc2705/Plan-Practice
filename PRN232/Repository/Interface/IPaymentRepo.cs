using BusinessObject.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IPaymentRepo
    {
        Task<Payment> CreatePayment(int userId, long orderCode, int packageId, int amount, string description);
        Task<Payment> GetPaymentByOrderCode(long orderCode);
        Task<Payment> GetPaymentById(int id);
        Task<List<Payment>> GetUserPayments(int userId);
        Task<Payment> UpdatePaymentStatus(long orderCode, string status, string transactionCode = null, string paymentLinkId = null);
        Task<bool> AddCoinToUser(int userId, int coinAmount);


        Task<List<Payment>> GetAllPaidPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<(List<Payment> payments, int totalCount)> GetPaginatedPaidPaymentsAsync(
            int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);

      
    }
}
