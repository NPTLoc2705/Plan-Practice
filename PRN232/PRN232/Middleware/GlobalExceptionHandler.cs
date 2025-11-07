using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PRN232.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var (statusCode, message) = exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                ArgumentNullException => (HttpStatusCode.BadRequest, exception.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Forbidden, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = GetTitle(statusCode),
                Detail = message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = (int)statusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = message,
                statusCode = (int)statusCode
            }, cancellationToken);

            return true;
        }

        private static string GetTitle(HttpStatusCode statusCode) => statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.Unauthorized => "Unauthorized",
            _ => "Internal Server Error"
        };
    }
}
