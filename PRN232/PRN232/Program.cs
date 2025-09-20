
using DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Interface;
using Repository.Method;
using Scalar.AspNetCore;
using Service;
using Service.JWT;
using Service.QuizzInterface;
using Service.QuizzMethod;
using System.Text;

namespace PRN232
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Allow CORS for frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4000", "https://writingaibeta002.aihubproduction.com")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });


            builder.Services.AddControllers();
            builder.Services.AddDbContext<PlantPraticeDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddOpenApi();

            //Injection of services and repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOtpVerifyRepository, OtpVerifyRepository>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJwtService, JWTService>();

            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
            builder.Services.AddScoped<IQuizResultRepository, QuizResultRepository>();
            builder.Services.AddScoped<IUserAnswerRepository, UserAnswerRepository>();

            builder.Services.AddScoped<IQuizManagementService, QuizManagementService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IQuizResultService, QuizResultService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IAnswerService, AnswerService>();
            builder.Services.AddScoped<IUserAnswerService, UserAnswerService>();

            var jwtKey = builder.Configuration["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(jwtKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            // Authorization
            builder.Services.AddAuthorization();

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
           


                var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");


            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
