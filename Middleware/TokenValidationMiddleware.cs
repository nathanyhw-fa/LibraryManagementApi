using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;  

namespace LibraryManagementSystem.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var _context = context.RequestServices.GetRequiredService<LibraryContext>();
            var sessionIdClaim = context.User?.FindFirst("sid");
            if (sessionIdClaim != null)
            {
                Guid sessionId = Guid.Parse(sessionIdClaim.Value);

                // Check if the session status and expiry date
                var session = await _context.SessionTokens
                    .FirstOrDefaultAsync(st => st.SessionId == sessionId);

                if (session == null || session.Status != 1 || session.ExpiryDateUTC < DateTime.UtcNow)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Session is no longer active.");
                    return;
                }
            }
            // Continue with the request
            await _next(context); 
        }
    }

}
