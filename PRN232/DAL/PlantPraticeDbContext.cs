using BusinessObject;
using BusinessObject.Lesson;
using BusinessObject.Lesson.Template;
using BusinessObject.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class PlantPraticeDbContext : DbContext
    {
        public PlantPraticeDbContext()
        {
        }

        public PlantPraticeDbContext(DbContextOptions<PlantPraticeDbContext> options)
         : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OtpVerify> OtpVerifies { get; set; }
        public DbSet<Quizs> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<LessonPlanner> LessonPlanners { get; set; }
        public DbSet<Class>Classes { get; set; }
        public DbSet<GradeLevel> GradeLevels { get; set; }

        public DbSet<QuizOTP> QuizOTPs { get; set; }

        public DbSet<QuizOTPAccess> QuizOTPAccesses {  get; set; }
        //TEST
        public DbSet<Unit> Units { get; set; }
        public DbSet<LessonDefinition> LessonDefinitions { get; set; }

        // ============================================
        // LESSON COMPONENTS (Join Tables)
        // ============================================
        public DbSet<LessonObjective> LessonObjectives { get; set; }
        public DbSet<LessonSkill> LessonSkills { get; set; }
        public DbSet<LessonAttitude> LessonAttitudes { get; set; }
        public DbSet<LessonLanguageFocus> LessonLanguageFocusItems { get; set; }
        public DbSet<LessonPreparation> LessonPreparations { get; set; }
        public DbSet<LessonActivityStage> LessonActivityStages { get; set; }
        public DbSet<LessonActivityItem> LessonActivityItems { get; set; }

        // ============================================
        // USER-OWNED TEMPLATE MODELS
        // ============================================
        public DbSet<ObjectiveTemplate> ObjectiveTemplates { get; set; }
        public DbSet<SkillTemplate> SkillTemplates { get; set; }
        public DbSet<AttitudeTemplate> AttitudeTemplates { get; set; }
        public DbSet<LanguageFocusType> LanguageFocusTypes { get; set; }
        public DbSet<MethodTemplate> MethodTemplates { get; set; }
        public DbSet<InteractionPattern> InteractionPatterns { get; set; }
        public DbSet<PreparationType> PreparationTypes { get; set; }
        public DbSet<ActivityTemplate> ActivityTemplates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(GetConnectionString());

        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["ConnectionStrings:DefaultConnection"];
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<LessonPlanner>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LessonNumber)
                    .HasColumnName("LessonNumber")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("timezone('utc', now())");

                // User relationship
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                // Optional relationships
                entity.HasOne(e => e.MethodTemplate)
                    .WithMany()
                    .HasForeignKey(e => e.MethodTemplateId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Quiz relationship - One LessonPlanner can have many Quizzes
                entity.HasMany(e => e.Quizzes)
                    .WithOne(q => q.LessonPlanner)
                    .HasForeignKey(q => q.LessonPlannerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_User_Email");
                entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("IX_User_Username");

                entity.Property(e => e.Createdat).HasColumnName("Createdat")
                    .HasDefaultValueSql("timezone('utc', now())");
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.Role)
                     .HasConversion<string>()
                     .HasDefaultValue(UserRole.Student)
                     .HasMaxLength(20);

                entity.Property(e => e.EmailVerified).HasDefaultValue(false);
                entity.Property(e => e.IsBanned).HasDefaultValue(false);
            });

            // OtpVerify entity configuration
            modelBuilder.Entity<OtpVerify>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnName("Createdat")
                    .HasDefaultValueSql("timezone('utc', now())");

                entity.Property(e => e.IsUsed).HasDefaultValue(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OtpVerifies)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => new { e.Email, e.Purpose, e.IsUsed })
                    .HasDatabaseName("IX_OtpVerify_Email_Purpose_IsUsed");

                entity.HasIndex(e => e.ExpiredAt)
                    .HasDatabaseName("IX_OtpVerify_ExpiredAt");
            });

            modelBuilder.Entity<QuizOTP>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.OTPCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("timezone('utc', now())");

                entity.Property(e => e.ExpiresAt)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.UsageCount)
                    .HasDefaultValue(0);

                // Quan hệ 1 QuizOTP - N QuizOTPAccess
                entity.HasMany(e => e.AccessLogs)
                    .WithOne(a => a.OTP)
                    .HasForeignKey(a => a.OTPId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ 1 User (teacher) - N QuizOTP
                entity.HasOne(e => e.CreatedByTeacher)
                    .WithMany() // nếu bạn không cần collection trong User
                    .HasForeignKey(e => e.CreatedByTeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ 1 Quiz - N QuizOTP
                entity.HasOne(e => e.Quiz)
                    .WithMany()
                    .HasForeignKey(e => e.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index để tăng tốc truy vấn tìm OTP còn hiệu lực
                entity.HasIndex(e => new { e.OTPCode, e.IsActive, e.ExpiresAt })
                    .HasDatabaseName("IX_QuizOTP_Code_IsActive_ExpiresAt");
            });


            // QuizOTPAccess entity configuration
            modelBuilder.Entity<QuizOTPAccess>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

         

                // Quan hệ N QuizOTPAccess - 1 QuizOTP
                entity.HasOne(e => e.OTP)
                    .WithMany(o => o.AccessLogs)
                    .HasForeignKey(e => e.OTPId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ N QuizOTPAccess - 1 User (Student)
                entity.HasOne(e => e.Student)
                    .WithMany() // nếu không có navigation trong User
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index giúp tìm theo OTP hoặc Student nhanh hơn
                entity.HasIndex(e => new { e.OTPId, e.StudentId })
                    .HasDatabaseName("IX_QuizOTPAccess_OTP_Student");

               
            });


            // Quiz relationships
            modelBuilder.Entity<Quizs>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // LessonPlanner relationship
                entity.HasOne(q => q.LessonPlanner)
                    .WithMany(lp => lp.Quizzes)
                    .HasForeignKey(q => q.LessonPlannerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Quizs>()
                .HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId);

            modelBuilder.Entity<QuizResult>()
                .HasOne(qr => qr.User)
                .WithMany(u => u.QuizResults)
                .HasForeignKey(qr => qr.UserId);

            modelBuilder.Entity<QuizResult>()
                .HasOne(qr => qr.Quiz)
                .WithMany(q => q.QuizResults)
                .HasForeignKey(qr => qr.QuizId);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.QuizResult)
                .WithMany(qr => qr.UserAnswers)
                .HasForeignKey(ua => ua.QuizResultId);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany()
                .HasForeignKey(ua => ua.QuestionId);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Answer)
                .WithMany()
                .HasForeignKey(ua => ua.AnswerId);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}