using BusinessObject;
using BusinessObject.Lesson;
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
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
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
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_User_Email");

                entity.HasIndex(e => e.Username)
                    .IsUnique()
                    .HasDatabaseName("IX_User_Username");

                entity.Property(e => e.Createdat).HasColumnName("Createdat")
                    .HasDefaultValueSql("timezone('utc', now())");

                entity.Property(e => e.Role)
                     .HasConversion<string>()
                     .HasDefaultValue(UserRole.Student)
                     .HasMaxLength(20);
                entity.Property(e => e.EmailVerified)
                    .HasDefaultValue(false);

            });
            modelBuilder.Entity<OtpVerify>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnName("Createdat")
                    .HasDefaultValueSql("timezone('utc', now())");

                entity.Property(e => e.IsUsed)
                    .HasDefaultValue(false);

                // Configure relationship with User
                entity.HasOne(d => d.User)
                    .WithMany(p => p.OtpVerifies)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Index for faster queries
                entity.HasIndex(e => new { e.Email, e.Purpose, e.IsUsed })
                    .HasDatabaseName("IX_OtpVerify_Email_Purpose_IsUsed");

                entity.HasIndex(e => e.ExpiredAt)
                    .HasDatabaseName("IX_OtpVerify_ExpiredAt");
                modelBuilder.Entity<Quiz>()
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

            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }

}
