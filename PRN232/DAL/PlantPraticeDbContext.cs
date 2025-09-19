using BusinessObject;
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

            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }

}
