using Microsoft.Extensions.Logging;
using Repository.Interface;
using Repository.Method;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Method
{
    public class CoinService : ICoinService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CoinService> _logger;
        private const int LESSON_GENERATION_COST = 50;

        public CoinService(IUserRepository userRepo, ILogger<CoinService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<bool> DeductCoinsForLessonGeneration(int userId)
        {
            try
            {
                var user = await _userRepo.GetUserById(userId);

                if (user == null)
                {
                    _logger.LogWarning("User not found: UserId={UserId}", userId);
                    return false;
                }

                if (user.CoinBalance < LESSON_GENERATION_COST)
                {
                    _logger.LogWarning("Insufficient coins: UserId={UserId}, Balance={Balance}, Required={Required}",
                        userId, user.CoinBalance, LESSON_GENERATION_COST);
                    return false;
                }

                user.CoinBalance -= LESSON_GENERATION_COST;
                await _userRepo.UpdateAsync(user);

                _logger.LogInformation("Coins deducted for lesson generation: UserId={UserId}, Amount={Amount}, NewBalance={NewBalance}",
                    userId, LESSON_GENERATION_COST, user.CoinBalance);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deducting coins for UserId={UserId}", userId);
                throw;
            }
        }
        public async Task<bool> RefundCoins(int userId, int amount)
        {
            try
            {
                var user = await _userRepo.GetUserById(userId);
                if (user == null)
                {
                    _logger.LogWarning("Cannot refund: User not found: UserId={UserId}", userId);
                    return false;
                }

                user.CoinBalance += amount;
                await _userRepo.UpdateAsync(user);

                _logger.LogInformation("Coins refunded: UserId={UserId}, Amount={Amount}, NewBalance={NewBalance}",
                    userId, amount, user.CoinBalance);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refunding coins for UserId={UserId}, Amount={Amount}", userId, amount);
                throw;
            }
        }
        public async Task<int> GetUserCoinBalance(int userId)
        {
            try
            {
                var user = await _userRepo.GetUserById(userId);
                return user?.CoinBalance ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coin balance for UserId={UserId}", userId);
                throw;
            }
        }
    }
}
