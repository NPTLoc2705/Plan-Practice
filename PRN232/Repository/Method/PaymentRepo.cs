using BusinessObject.Payments;
using DAL;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class PaymentRepo : IPaymentRepo
    {
        private readonly PaymentDAO _paymentDAO;

        public PaymentRepo(PaymentDAO paymentDAO)
        {
            _paymentDAO = paymentDAO;
        }

        public async Task<bool> AddCoinToUser(int userId, int coinAmount)
        => await _paymentDAO.AddCoinToUser(userId, coinAmount);

        public async Task<Payment> CreatePayment(int userId, long orderCode, int packageId ,int amount, string description)
        => await _paymentDAO.CreatePayment(userId, orderCode, packageId ,amount, description);

        public async Task<Payment> GetPaymentById(int id)
        => await _paymentDAO.GetPaymentById(id);

        public async Task<Payment> GetPaymentByOrderCode(long orderCode)
        => await _paymentDAO.GetPaymentByOrderCode(orderCode);

        public async Task<List<Payment>> GetUserPayments(int userId)
        => await _paymentDAO.GetUserPayments(userId);

        public async Task<Payment> UpdatePaymentStatus(long orderCode, string status, string transactionCode = null, string paymentLinkId = null)
       => await _paymentDAO.UpdatePaymentStatus(orderCode, status, transactionCode, paymentLinkId);
    }
}
