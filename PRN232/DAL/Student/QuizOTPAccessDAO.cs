using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Student
{
    public class QuizOTPAccessDAO
    {
        private readonly PlantPraticeDbContext _context;

        public QuizOTPAccessDAO(PlantPraticeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<QuizOTPAccess> GetByIdAsync(int id)
        {
            return await _context.QuizOTPAccesses
                .Include(a => a.OTP)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<QuizOTPAccess>> GetAllAsync()
        {
            return await _context.QuizOTPAccesses
                .Include(a => a.OTP)
                .Include(a => a.Student)
                .ToListAsync();
        }

        public async Task<QuizOTPAccess> CreateAsync(QuizOTPAccess access)
        {
            _context.QuizOTPAccesses.Add(access);
            await _context.SaveChangesAsync();
            return access;
        }

        public async Task<IEnumerable<QuizOTPAccess>> GetByOTPIdAsync(int otpId)
        {
            return await _context.QuizOTPAccesses
                .Include(a => a.Student)
                .Where(a => a.OTPId == otpId)
                .OrderByDescending(a => a.AccessedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizOTPAccess>> GetByStudentIdAsync(int studentId)
        {
            return await _context.QuizOTPAccesses
                .Include(a => a.OTP)
                    .ThenInclude(o => o.Quiz)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.AccessedAt)
                .ToListAsync();
        }

        public async Task<bool> HasStudentAccessedOTPAsync(int otpId, int studentId)
        {
            return await _context.QuizOTPAccesses
                .AnyAsync(a => a.OTPId == otpId && a.StudentId == studentId);
        }

        public async Task<int> GetAccessCountByOTPIdAsync(int otpId)
        {
            return await _context.QuizOTPAccesses
                .CountAsync(a => a.OTPId == otpId);
        }

        public async Task<QuizOTPAccess> GetLatestAccessAsync(int otpId, int studentId)
        {
            return await _context.QuizOTPAccesses
                .Where(a => a.OTPId == otpId && a.StudentId == studentId)
                .OrderByDescending(a => a.AccessedAt)
                .FirstOrDefaultAsync();
        }
    }
}

