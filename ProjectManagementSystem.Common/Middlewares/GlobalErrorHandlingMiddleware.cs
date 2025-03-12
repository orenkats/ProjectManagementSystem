
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProjectManagementSystem.Common.Exceptions;

namespace ProjectManagementSystem.Common.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            switch (exception)
            {
                case BaseException baseException:
                    response.StatusCode = baseException.HttpStatusCode;
                    _logger.LogError($"[ERROR] {baseException.GetType().Name}: {baseException.Message}");
                    break;

                // case UserNotFoundException _:
                //     response.StatusCode = StatusCodes.Status404NotFound;
                //     _logger.LogError("[ERROR] UserNotFoundException: User not found.");
                //     break;
                //
                // case NotAuthorizedException _:
                //     response.StatusCode = StatusCodes.Status401Unauthorized;
                //     _logger.LogError("[ERROR] NotAuthorizedException: Invalid credentials or unauthorized access.");
                //     break;
                //
                // case PasswordResetRequiredException _:
                //     response.StatusCode = StatusCodes.Status403Forbidden;
                //     _logger.LogError("[ERROR] PasswordResetRequiredException: Password reset is required.");
                //     break;
                //
                // case UserNotConfirmedException _:
                //     response.StatusCode = StatusCodes.Status403Forbidden;
                //     _logger.LogError("[ERROR] UserNotConfirmedException: User account is not confirmed.");
                //     break;

                default:
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    _logger.LogError($"[ERROR] Unexpected Error: {exception.Message}");
                    break;
            }

            await context.Response.WriteAsJsonAsync(new { error = exception.Message });
        }
    }
}
