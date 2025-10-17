
using DAL;
using DAL.LessonDAO;
using DAL.QuizDAO;
using DAL.Student;
using DAL.LessonDAO.Template;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Interface;
using Repository.Interface.Student;
using Repository.Method;
using Repository.Method.Student;
using Scalar.AspNetCore;
using Service.Interface;
using Service.JWT;
using Service.Method;
using Service.QuizzInterface;
using Service.QuizzMethod;
using Services;
using System.Text;
using Repository.Interface.Template;
using Repository.Method.Template;
using Service.Interface.Template;
using Service.Method.Template;

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

            //LessonPlanner Related Shitzzz

            builder.Services.AddScoped<SkillTypeDAO>();
            builder.Services.AddScoped<ISkillTypeRepository, SkillTypeRepository>();
            builder.Services.AddScoped<ISkillTypeService, SkillTypeService>();

            builder.Services.AddScoped<LessonPlannerDAO>();
            builder.Services.AddScoped<ILessonPlannerRepository, LessonPlannerRepository>();
            builder.Services.AddScoped<ILessonPlannerService, LessonPlannerService>();

            builder.Services.AddScoped<GradeLevelDAO>();
            builder.Services.AddScoped<IGradeLevelRepository, GradeLevelRepository>();
            builder.Services.AddScoped<IGradeLevelService, GradeLevelService>();

            builder.Services.AddScoped<ClassDAO>();
            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            builder.Services.AddScoped<IClassService, ClassService>();

            builder.Services.AddScoped<AttitudeTemplateDAO>();
            builder.Services.AddScoped<IAttitudeTemplateRepository, AttitudeTemplateRepository>();
            builder.Services.AddScoped<IAttitudeTemplateService, AttitudeTemplateService>();

            builder.Services.AddScoped<InteractionPatternDAO>();
            builder.Services.AddScoped<IInteractionPatternRepository, InteractionPatternRepository>();
            builder.Services.AddScoped<IInteractionPatternService, InteractionPatternService>();

            builder.Services.AddScoped<LanguageFocusTypeDAO>();
            builder.Services.AddScoped<ILanguageFocusTypeRepository, LanguageFocusTypeRepository>();
            builder.Services.AddScoped<ILanguageFocusTypeService, LanguageFocusTypeService>();

            builder.Services.AddScoped<ObjectiveTemplateDAO>();
            builder.Services.AddScoped<IObjectiveTemplateRepository, ObjectiveTemplateRepository>();
            builder.Services.AddScoped<IObjectiveTemplateService, ObjectiveTemplateService>();

            builder.Services.AddScoped<PreparationTypeDAO>();
            builder.Services.AddScoped<IPreparationTypeRepository, PreparationTypeRepository>();
            builder.Services.AddScoped<IPreparationTypeService, PreparationTypeService>();

            builder.Services.AddScoped<SkillTemplateDAO>();
            builder.Services.AddScoped<ISkillTemplateRepository, SkillTemplateRepository>();
            builder.Services.AddScoped<ISkillTemplateService, SkillTemplateService>();
            //Injection of DAO

            builder.Services.AddScoped<QuizDAO>();
            builder.Services.AddScoped<QuestionDAO>();
            builder.Services.AddScoped<AnswerDAO>();
            builder.Services.AddScoped<QuizResultDAO>();
            builder.Services.AddScoped<UserAnswerDAO>();
            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<OtpVerifyDAO>();
            builder.Services.AddScoped<AdminDAO>();

            //Student
            builder.Services.AddScoped<AnswerStudentDAO>();
            builder.Services.AddScoped<QuestionStudentDAO>();
            builder.Services.AddScoped<QuizResultStudentDAO>();
            builder.Services.AddScoped<QuizStatisticsStudentDAO>();
            builder.Services.AddScoped<QuizStudentDAO>();
            builder.Services.AddScoped<UserAnswerStudentDAO>();
            //Quiz otp
            builder.Services.AddScoped<QuizOTPDAO>();
            builder.Services.AddScoped<QuizOTPAccessDAO>();





            //Injection of services and repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOtpVerifyRepository, OtpVerifyRepository>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IJwtService, JWTService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
            builder.Services.AddScoped<IQuizResultRepository, QuizResultRepository>();
            builder.Services.AddScoped<IUserAnswerRepository, UserAnswerRepository>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();



            //Student
            builder.Services.AddScoped<IQuizStudentRepository, QuizStudentRepository>();
            builder.Services.AddScoped<IQuestionStudentRepository, QuestionStudentRepository>();
            builder.Services.AddScoped<IAnswerStudentRepository, AnswerStudentRepository>();
            builder.Services.AddScoped<IQuizResultStudentRepository, QuizResultStudentRepository>();
            builder.Services.AddScoped<IUserAnswerStudentRepository, UserAnswerStudentRepository>();
            builder.Services.AddScoped<IQuizStatisticsStudentRepository, QuizStatisticsStudentRepository>();

            //Quiz OTP
            builder.Services.AddScoped<IQuizOTPRepository, QuizOTPRepository>();
            builder.Services.AddScoped<IQuizOTPAccessRepository, QuizOTPAccessRepository>();


            
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IQuizResultService, QuizResultService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IAnswerService, AnswerService>();
            builder.Services.AddScoped<IUserAnswerService, UserAnswerService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            //  builder.Services.AddScoped<IStudentQuizService, StudentQuizService>();
            builder.Services.AddScoped<IStudentQuizService, StudentQuizService>();
            builder.Services.AddScoped<IQuizOTPService, QuizOTPService>();


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
            builder.Services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

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
