using BusinessObject;
using BusinessObject.Lesson;
using BusinessObject.Lesson.Template;
using BusinessObject.Payments;
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
        public DbSet<LessonPlannerDocument> LessonPlannerDocuments { get; set; }
        public DbSet<Class>Classes { get; set; }
        public DbSet<GradeLevel> GradeLevels { get; set; }

        public DbSet<QuizOTP> QuizOTPs { get; set; }

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


        // ============================================
        // Payment
        // ============================================
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Package> Packages { get; set; }
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
                    .OnDelete(DeleteBehavior.SetNull);
                    
                // Optional relationships
                entity.HasOne(e => e.MethodTemplate)
                    .WithMany()
                    .HasForeignKey(e => e.MethodTemplateId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Quiz relationship - One LessonPlanner can have many Quizzes (Required)
                entity.HasMany(e => e.Quizzes)
                    .WithOne(q => q.LessonPlanner)
                    .HasForeignKey(q => q.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Restrict)  // Changed from SetNull to Restrict
                    .IsRequired();  // Added to make it required
            });

            // LessonPlannerDocument entity configuration
            modelBuilder.Entity<LessonPlannerDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FilePath)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("timezone('utc', now())");

                entity.HasOne(e => e.LessonPlanner)
                    .WithMany()
                    .HasForeignKey(e => e.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.LessonPlannerId)
                    .HasDatabaseName("IX_LessonPlannerDocument_LessonPlannerId");
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
                entity.Property(e => e.CoinBalance)
               .HasDefaultValue(100);

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


            


            // Quiz relationships
            modelBuilder.Entity<Quizs>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // LessonPlanner relationship - Make it required (non-nullable)
                entity.HasOne(q => q.LessonPlanner)
                    .WithMany(lp => lp.Quizzes)
                    .HasForeignKey(q => q.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Cascade)  // Changed from SetNull to Restrict
                    .IsRequired();  // Added to make it required
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

            // ============================================
            // LESSON PLANNER STEP COMPONENTS CONFIGURATION
            // ============================================

            // LessonObjective - Set null on template delete
            modelBuilder.Entity<LessonObjective>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.LessonPlanner)
                    .WithMany(lp => lp.Objectives)
                    .HasForeignKey(e => e.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ObjectiveTemplate)
                    .WithMany()
                    .HasForeignKey(e => e.ObjectiveTemplateId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            // LessonSkill - Set null on template delete
            modelBuilder.Entity<LessonSkill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.LessonPlanner)
                    .WithMany(lp => lp.Skills)
                    .HasForeignKey(e => e.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SkillTemplate)
                    .WithMany()
                    .HasForeignKey(e => e.SkillTemplateId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            // LessonAttitude - Set null on template delete
            modelBuilder.Entity<LessonAttitude>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.LessonPlanner)
                    .WithMany(lp => lp.Attitudes)
                    .HasForeignKey(e => e.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AttitudeTemplate)
                    .WithMany()
                    .HasForeignKey(e => e.AttitudeTemplateId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            // LessonLanguageFocus - Set null on template delete
            modelBuilder.Entity<LessonLanguageFocus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.LessonPlanner)
                    .WithMany(lp => lp.LanguageFocusItems)
                    .HasForeignKey(e => e.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.LanguageFocusType)
                    .WithMany()
                    .HasForeignKey(e => e.LanguageFocusTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            // LessonPreparation - Set null on template delete
            modelBuilder.Entity<LessonPreparation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.LessonPlanner)
                    .WithMany(lp => lp.Preparations)
                    .HasForeignKey(e => e.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PreparationType)
                    .WithMany()
                    .HasForeignKey(e => e.PreparationTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            // LessonActivityStage
            modelBuilder.Entity<LessonActivityStage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.LessonPlanner)
                    .WithMany(lp => lp.ActivityStages)
                    .HasForeignKey(e => e.LessonPlannerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // LessonActivityItem - Set null on template delete
            modelBuilder.Entity<LessonActivityItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.LessonActivityStage)
                    .WithMany(stage => stage.ActivityItems)
                    .HasForeignKey(e => e.LessonActivityStageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.InteractionPattern)
                    .WithMany()
                    .HasForeignKey(e => e.InteractionPatternId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                entity.HasOne(e => e.ActivityTemplate)
                    .WithMany()
                    .HasForeignKey(e => e.ActivityTemplateId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            //Payment entity configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Amount)
                  
                    .IsRequired();
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("timezone('utc', now())");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Package)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(e => e.PackageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            //Package entity configuration

            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CoinAmount).IsRequired();
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                
                 
            });
            SeedPackages(modelBuilder);
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
            public static void SeedPackages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Package>().HasData(
                new Package
                {
                    Id = 1,
                    Name = "Starter Pack",
                    CoinAmount = 100,
                    Price = 9900, // $9.99 or 99,000 VND
                    Description = "Perfect for getting started with lesson creation",
                    IsActive = true,
                    
                },
                new Package
                {
                    Id = 2,
                    Name = "Pro Creator",
                    CoinAmount = 500,
                    Price = 39900, // $39.99 or 399,000 VND
                    Description = "For frequent lesson creators",
                    IsActive = true,
                   
                },
                new Package
                {
                    Id = 3,
                    Name = "Power User",
                    CoinAmount = 1000,
                    Price = 69900, // $69.99 or 699,000 VND
                    Description = "Maximum value for power users",
                    IsActive = true,
                   
                }
            );
        }
    }
}