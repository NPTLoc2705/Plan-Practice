using BusinessObject;
using BusinessObject.Dtos.Quiz;
using Repository.Interface;
using Repository.Interface.Student;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Method
{
    public class QuizOTPService : IQuizOTPService
    {
        private readonly IQuizOTPRepository _otpRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IUserRepository _userRepository;

        public QuizOTPService(
            IQuizOTPRepository otpRepository,
            IQuizRepository quizRepository,
            IUserRepository userRepository)
        {
            _otpRepository = otpRepository;
            _quizRepository = quizRepository;
            _userRepository = userRepository;
        }

        public async Task<QuizOTPDto> GenerateOTPAsync(int quizId, int teacherId, GenerateOTPDto dto)
        {
            // Validate quiz exists and belongs to teacher
            var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
            if (quiz == null)
                throw new InvalidOperationException($"Quiz with ID {quizId} not found");

             if (quiz.LessonPlannerId != teacherId)
                 throw new UnauthorizedAccessException("You don't have permission to generate OTP for this quiz");

            // Generate unique OTP code
            string otpCode;
            do
            {
                otpCode = GenerateRandomOTP();
            }
            while (await _otpRepository.IsOTPCodeExistsAsync(otpCode));

            // Create OTP entity
            var quizOTP = new QuizOTP
            {
                OTPCode = otpCode,
                QuizId = quizId,
                CreatedByTeacherId = teacherId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(dto.ExpiryMinutes),
                IsActive = true,
                UsageCount = 0,
                MaxUsage = dto.MaxUsage
            };

            // Save to database
            var createdOTP = await _otpRepository.CreateAsync(quizOTP);

            // Get teacher info
            var teacher = await _userRepository.GetUserById(teacherId);

            // Return DTO
            return new QuizOTPDto
            {
                Id = createdOTP.Id,
                OTPCode = createdOTP.OTPCode,
                QuizId = createdOTP.QuizId,
                QuizTitle = quiz.Title,
                TeacherName = teacher?.Username ?? "Unknown",
                CreatedAt = createdOTP.CreatedAt,
                ExpiresAt = createdOTP.ExpiresAt,
                IsActive = createdOTP.IsActive,
                UsageCount = createdOTP.UsageCount,
                MaxUsage = createdOTP.MaxUsage
            };
        }

        public async Task<IEnumerable<QuizOTPDto>> GetTeacherOTPsAsync(int teacherId)
        {
            var otps = await _otpRepository.GetByTeacherIdAsync(teacherId);

            return otps.Select(o => new QuizOTPDto
            {
                Id = o.Id,
                OTPCode = o.OTPCode,
                QuizId = o.QuizId,
                QuizTitle = o.Quiz?.Title ?? "Unknown Quiz",
                TeacherName = o.CreatedByTeacher?.Username ?? "Unknown",
                CreatedAt = o.CreatedAt,
                ExpiresAt = o.ExpiresAt,
                IsActive = o.IsActive,
                UsageCount = o.UsageCount,
                MaxUsage = o.MaxUsage
            });
        }

        public async Task<IEnumerable<QuizOTPDto>> GetQuizOTPsAsync(int quizId, int teacherId)
        {
            var otps = await _otpRepository.GetByQuizIdAsync(quizId);

            // Filter by teacher if needed
            otps = otps.Where(o => o.CreatedByTeacherId == teacherId);

            return otps.Select(o => new QuizOTPDto
            {
                Id = o.Id,
                OTPCode = o.OTPCode,
                QuizId = o.QuizId,
                QuizTitle = o.Quiz?.Title ?? "Unknown Quiz",
                TeacherName = o.CreatedByTeacher?.Username ?? "Unknown",
                CreatedAt = o.CreatedAt,
                ExpiresAt = o.ExpiresAt,
                IsActive = o.IsActive,
                UsageCount = o.UsageCount,
                MaxUsage = o.MaxUsage
            });
        }

        public async Task<bool> RevokeOTPAsync(int otpId, int teacherId)
        {
            var otp = await _otpRepository.GetByIdAsync(otpId);

            if (otp == null)
                return false;

            if (otp.CreatedByTeacherId != teacherId)
                throw new UnauthorizedAccessException("You don't have permission to revoke this OTP");

            otp.IsActive = false;
            await _otpRepository.UpdateAsync(otp);

            return true;
        }

        public async Task<OTPValidationResultDto> ValidateOTPAsync(string otpCode, int studentId)
        {
            // Get active OTP
            var otp = await _otpRepository.GetActiveOTPByCodeAsync(otpCode);

            if (otp == null)
            {
                return new OTPValidationResultDto
                {
                    IsValid = false,
                    Message = "Invalid OTP code"
                };
            }

            // Check expiry
            if (DateTime.UtcNow > otp.ExpiresAt)
            {
                otp.IsActive = false;
                await _otpRepository.UpdateAsync(otp);

                return new OTPValidationResultDto
                {
                    IsValid = false,
                    Message = "OTP has expired"
                };
            }

            // Check max usage
            if (otp.MaxUsage.HasValue && otp.UsageCount >= otp.MaxUsage.Value)
            {
                return new OTPValidationResultDto
                {
                    IsValid = false,
                    Message = "OTP has reached maximum usage limit"
                };
            }

            

            // Update usage count
            otp.UsageCount++;
            await _otpRepository.UpdateAsync(otp);

            // Prepare quiz data (without correct answers)
            var quizDto = new QuizDetailDto
            {
                Id = otp.Quiz.Id,
                Title = otp.Quiz.Title,
                Description = otp.Quiz.Description,
                Questions = otp.Quiz.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Content = q.Content,
                    Answers = q.Answers.Select(a => new AnswerOptionDto
                    {
                        Id = a.Id,
                        Content = a.Content
                    }).ToList()
                }).ToList()
            };

            return new OTPValidationResultDto
            {
                IsValid = true,
                Message = "OTP validated successfully",
                Quiz = quizDto
            };
        }

        public async Task<QuizDetailDto> GetQuizByOTPAsync(string otpCode, int studentId)
        {
            var validationResult = await ValidateOTPAsync(otpCode, studentId);

            if (!validationResult.IsValid)
                throw new InvalidOperationException(validationResult.Message);

            return validationResult.Quiz;
        }

        

        public async Task<int> CleanupExpiredOTPsAsync()
        {
            return await _otpRepository.DeactivateExpiredOTPsAsync();
        }

        public async Task<bool> ExtendOTPExpiryAsync(int otpId, int teacherId, int additionalMinutes)
        {
            var otp = await _otpRepository.GetByIdAsync(otpId);

            if (otp == null)
                return false;

            if (otp.CreatedByTeacherId != teacherId)
                throw new UnauthorizedAccessException("You don't have permission to extend this OTP");

            otp.ExpiresAt = otp.ExpiresAt.AddMinutes(additionalMinutes);

            // Reactivate if it was expired
            if (!otp.IsActive && DateTime.UtcNow < otp.ExpiresAt)
            {
                otp.IsActive = true;
            }

            await _otpRepository.UpdateAsync(otp);
            return true;
        }

        public async Task<QuizOTPDto> RegenerateOTPAsync(int otpId, int teacherId)
        {
            var oldOTP = await _otpRepository.GetByIdAsync(otpId);

            if (oldOTP == null)
                throw new InvalidOperationException("OTP not found");

            if (oldOTP.CreatedByTeacherId != teacherId)
                throw new UnauthorizedAccessException("You don't have permission to regenerate this OTP");

            // Deactivate old OTP
            oldOTP.IsActive = false;
            await _otpRepository.UpdateAsync(oldOTP);

            // Generate new OTP with same settings
            var dto = new GenerateOTPDto
            {
                QuizId = oldOTP.QuizId,
                ExpiryMinutes = (int)(oldOTP.ExpiresAt - oldOTP.CreatedAt).TotalMinutes,
                MaxUsage = oldOTP.MaxUsage
            };

            return await GenerateOTPAsync(oldOTP.QuizId, teacherId, dto);
        }

        private string GenerateRandomOTP()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new StringBuilder(6);

            for (int i = 0; i < 6; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}