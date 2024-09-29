using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using log4net;

namespace LibraryManagementSystem.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILog log = LogManager.GetLogger(typeof(LoggingMiddleware));

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log the request body
            context.Request.EnableBuffering();
            var requestBody = await ReadRequestBodyAsync(context.Request);
            log.Info($"Request: {context.Request.Method} {context.Request.Path}\nBody: {requestBody}");

            // Capture the original response body stream
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                // Replace the response body stream to capture it
                context.Response.Body = responseBody;

                // Call the next middleware
                await _next(context); 

                // Log the response body
                var responseBodyText = await ReadResponseBodyAsync(context.Response);
                log.Info($"Response: {context.Response.StatusCode}\nBody: {responseBodyText}");

                // Copy the contents of the response body back to the original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Position = 0;
            using (var reader = new StreamReader(request.Body, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                // Reset stream position for next middleware
                request.Body.Position = 0; 
                return body;
            }
        }

        private async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(response.Body, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                // Reset stream position for next middleware
                response.Body.Seek(0, SeekOrigin.Begin);
                return body;
            }
        }
    }
}
