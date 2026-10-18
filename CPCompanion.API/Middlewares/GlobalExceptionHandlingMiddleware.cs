
using CPCompanion.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace CPCompanion.API.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
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
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                ConflictException => (int)HttpStatusCode.Conflict,
                UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                NotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError           
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new { error = exception.Message };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
