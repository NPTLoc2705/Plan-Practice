
using DAL;
using DAL.QuizDAO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
                    policy.WithOrigins("http://localhost:4000", "https://localhost:4000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });


            builder.Services.AddControllers();
            builder.Services.AddDbContext<PlantPraticeDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });
            //Injection of DAO

            builder.Services.AddScoped<QuizDAO>();
            builder.Services.AddScoped<QuestionDAO>();
            builder.Services.AddScoped<AnswerDAO>();
            builder.Services.AddScoped<QuizResultDAO>();
            builder.Services.AddScoped<UserAnswerDAO>();



            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<OtpVerifyDAO>();
            //builder.Services.AddScoped<AnswerStudentDAO>();
            //builder.Services.AddScoped<QuestionStudentDAO>();
            //builder.Services.AddScoped<QuizResultStudentDAO>();
            //builder.Services.AddScoped<QuizStatisticsStudentDAO>();
            //builder.Services.AddScoped<QuizStudentDAO>();
            //builder.Services.AddScoped<UserAnswerStudentDAO>();





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
            //Student
            //builder.Services.AddScoped<IQuizStudentRepository, QuizStudentRepository>();
            //builder.Services.AddScoped<IQuestionStudentRepository, QuestionStudentRepository>();
            //builder.Services.AddScoped<IAnswerStudentRepository, AnswerStudentRepository>();
            //builder.Services.AddScoped<IQuizResultStudentRepository, QuizResultStudentRepository>();
            //builder.Services.AddScoped<IUserAnswerStudentRepository, UserAnswerStudentRepository>();
            //builder.Services.AddScoped<IQuizStatisticsStudentRepository, QuizStatisticsStudentRepository>();

            
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IQuizResultService, QuizResultService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IAnswerService, AnswerService>();
            builder.Services.AddScoped<IUserAnswerService, UserAnswerService>();

          //  builder.Services.AddScoped<IStudentQuizService, StudentQuizService>();
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
    internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        private readonly Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public BearerSecuritySchemeTransformer(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == JwtBearerDefaults.AuthenticationScheme))
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
                document.Components.SecuritySchemes["Bearer"] = securityScheme;

                // Modify to only apply security to endpoints with [Authorize]
                foreach (var path in document.Paths)
                {
                    foreach (var operation in path.Value.Operations)
                    {
                        // Check if the operation has an Authorize attribute (simplified check)
                        // You may need to customize this logic based on your needs
                        if (operation.Value.Extensions.TryGetValue("x-require-auth", out var auth) && auth.ToString() == "true")
                        {
                            operation.Value.Security ??= new List<OpenApiSecurityRequirement>();
                            operation.Value.Security.Add(new OpenApiSecurityRequirement
                            {
                                [securityScheme] = Array.Empty<string>()
                            });
                        }
                    }
                }
            }
        }
    }
}
