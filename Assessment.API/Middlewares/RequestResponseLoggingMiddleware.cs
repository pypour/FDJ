using System.Text;
using System.Text.Json;

namespace Assessment.API
{
    public class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, LogLevel? level)
    {
        public async Task Invoke(HttpContext context)
        {
            var startTime = DateTime.Now;
            context.Request.EnableBuffering();

            var request = await FormatRequestAndRewind(context.Request);
            await FormatResponse(context.Response);

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                // ...and use that for the temporary response body
                context.Response.Body = responseBody;

                try
                {
	                await next(context);
                }
                catch (Exception ex)
				{
					context.Response.Body = originalBodyStream;
					Log(context, startTime, request, ex.Message);

					throw;
                }

                var response = await FormatResponse(context.Response);

                await responseBody.CopyToAsync(originalBodyStream);

                Log(context, startTime, request, response);
            }
        }

        private void Log(HttpContext context, DateTime startTime, string request, string response)
        {
	        var endTime = DateTime.Now;
	        var totalMilliseconds = endTime.Subtract(startTime).TotalMilliseconds;
            var logData = new { TotalMilliseconds = totalMilliseconds, startTime, endTime, request, response };
            logger.Log(level ?? LogLevel.Information, JsonSerializer.Serialize(logData));
        }

        private async Task<string> FormatRequestAndRewind(HttpRequest request)
        {
            string bodyAsText;
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyAsText = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            return $"{request.Scheme} {request.Host}{request.Path}{request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            var resp = string.Empty;
            if (response.Body.CanSeek)
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                var length = (int) (response.Body.Length > 2000 ? 2000 : response.Body.Length);
                var chars = new char[length];
                await new StreamReader(response.Body).ReadAsync(chars, 0, length);
                
                response.Body.Seek(0, SeekOrigin.Begin);
                return $"{response.StatusCode}: {new string (chars)}";
            }
            return resp;

        }
    }
}