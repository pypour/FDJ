using Assessment.Contract.Common;
using Assessment.Contract.Enums;
using System.Text.Json;

namespace Assessment.API
{
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError($"Unhandled exception: {ex}");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;

                var response = JsonSerializer.Serialize(new ApiResult<object>(ExceptionCodeEnum.UnknownError), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await context.Response.WriteAsync(response);
            }
        }

    }
}
