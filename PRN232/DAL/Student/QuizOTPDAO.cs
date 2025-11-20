using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Student
{
    public class QuizOTPDAO
    {
        private readonly PlantPraticeDbContext _context;
        public QuizOTPDAO(PlantPraticeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<QuizOTP> GetByIdAsync(int id)
        {
            return await _context.QuizOTPs
                .Include(o => o.Quiz)
                .Include(o => o.CreatedByTeacher)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<QuizOTP> GetByOTPCodeAsync(string otpCode)
        {
            return await _context.QuizOTPs
                .Include(o => o.Quiz)
                    .ThenInclude(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(o => o.CreatedByTeacher)
                .FirstOrDefaultAsync(o => o.OTPCode == otpCode.ToUpper());
        }

        public async Task<IEnumerable<QuizOTP>> GetAllAsync()
        {
            return await _context.QuizOTPs
                .Include(o => o.Quiz)
                .Include(o => o.CreatedByTeacher)
                .ToListAsync();
        }

        public async Task<QuizOTP> CreateAsync(QuizOTP quizOTP)
        {
            _context.QuizOTPs.Add(quizOTP);
            await _context.SaveChangesAsync();
            return quizOTP;
        }

        public async Task<QuizOTP> UpdateAsync(QuizOTP quizOTP)
        {
            _context.QuizOTPs.Update(quizOTP);
            await _context.SaveChangesAsync();
            return quizOTP;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var otp = await _context.QuizOTPs.FindAsync(id);
            if (otp == null) return false;

            _context.QuizOTPs.Remove(otp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QuizOTP>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.QuizOTPs
                .Include(o => o.Quiz)
                .Include(o => o.CreatedByTeacher)
                .Where(o => o.CreatedByTeacherId == teacherId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizOTP>> GetByQuizIdAsync(int quizId)
        {
            return await _context.QuizOTPs
                .Include(o => o.CreatedByTeacher)
                .Where(o => o.QuizId == quizId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizOTP>> GetActiveOTPsAsync()
        {
            return await _context.QuizOTPs
                .Include(o => o.Quiz)
                .Include(o => o.CreatedByTeacher)
                .Where(o => o.IsActive && o.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<bool> IsOTPCodeExistsAsync(string otpCode)
        {
            return await _context.QuizOTPs
                .AnyAsync(o => o.OTPCode == otpCode.ToUpper() && o.IsActive);
        }

        public async Task<QuizOTP> GetActiveOTPByCodeAsync(string otpCode)
        {
            return await _context.QuizOTPs
                .Include(o => o.Quiz)
                    .ThenInclude(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(o => o.CreatedByTeacher)
                .FirstOrDefaultAsync(o =>
                    o.OTPCode == otpCode.ToUpper() &&
                    o.IsActive &&
                    o.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<IEnumerable<QuizOTP>> GetExpiredOTPsAsync()
        {
            return await _context.QuizOTPs
                .Where(o => o.ExpiresAt <= DateTime.UtcNow && o.IsActive)
                .ToListAsync();
        }

        public async Task<int> DeactivateExpiredOTPsAsync()
        {
            var expiredOTPs = await GetExpiredOTPsAsync();
            foreach (var otp in expiredOTPs)
            {
                otp.IsActive = false;
            }
            return await _context.SaveChangesAsync();
        }

        public async Task DeleteExpiredOtpAsynce()
        {
            var now = DateTime.UtcNow;
            var expire = _context.QuizOTPs.Where(q => q.ExpiresAt <= now);

            _context.QuizOTPs.RemoveRange(expire);
            await _context.SaveChangesAsync();
        }
    }
}

