using BusinessObject.Dtos.paymentDto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.payOS.Types;

namespace Service.Interface
{
    public interface IPaymentService
    {
        Task<CreatePaymentResponse> CreateCoinPayment(int userId, int packageId, string description);
        //Task<CreatePaymentResponse> CreateVipPayment(int userId, string description);
        Task<PaymentStatusResponse> GetPaymentStatus(long orderCode);
        Task<List<PaymentStatusResponse>> GetUserPaymentHistory(int userId);
        Task<bool> HandlePaymentWebhook(WebhookType webhookBody);
        Task<bool> UpgradeUserIfPaymentExists(int userId);
    }
}
