using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Method;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzMethod
{
    public class QuizOtpCleanupService : BackgroundService

    {
        private readonly IServiceProvider _provider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        public QuizOtpCleanupService(IServiceProvider provider)
        {
            _provider = provider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using var scope = _provider.CreateAsyncScope();
                var otpService = scope.ServiceProvider.GetRequiredService<IQuizOTPService>();

                try
                {
                    await otpService.CleanupExpiredOtpsAsync();
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
