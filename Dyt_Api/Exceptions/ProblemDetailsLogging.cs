namespace Dyt_Api.Exceptions
{
    public static class ProblemDetailsLogging
    {
        private const string LoggerCategory = "Dyt_Api.ProblemDetails";

        // 5xx responses are logged with their exception by GlobalExceptionHandler.
        public static void LogClientError(ProblemDetailsContext context)
        {
            var httpContext = context.HttpContext;
            var status = context.ProblemDetails.Status ?? httpContext.Response.StatusCode;

            if (status is < 400 or >= 500)
            {
                return;
            }

            var logger = httpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(LoggerCategory);

            var level = status is StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden
                ? LogLevel.Warning
                : LogLevel.Information;

            logger.Log(
                level,
                "Request {Method} {Path} returned {StatusCode}.",
                httpContext.Request.Method,
                httpContext.Request.Path,
                status);
        }
    }
}
