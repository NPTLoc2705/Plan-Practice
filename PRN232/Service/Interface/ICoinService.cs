using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICoinService
    {
        Task<bool> DeductCoinsForLessonGeneration(int userId);
        Task<int> GetUserCoinBalance(int userId);
        Task<bool> RefundCoins(int userId, int amount);
    }
}
