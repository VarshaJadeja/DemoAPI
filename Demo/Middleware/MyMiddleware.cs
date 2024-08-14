using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Demo.Middleware
{
    public class MyMiddleware
    {
        private readonly RequestDelegate _next;

        public MyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            Console.WriteLine("Hello Custom Middleware ");
            string operation = "";
            if (httpContext.Request.Method == HttpMethods.Get)
            {
                operation = "Get";
            }
            else if (httpContext.Request.Method == HttpMethods.Post)
            {
                operation = "Create";
            }
            else if (httpContext.Request.Method == HttpMethods.Put)
            {
                operation = "Update";
            }
            else if (httpContext.Request.Method == HttpMethods.Delete)
            {
                operation = "Delete";
            }

            if (!string.IsNullOrEmpty(operation))
            {
                Console.WriteLine($"CRUD Operation: {operation} - {httpContext.Request.Path}");
            }

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyMiddleware>();
        }
    }
}
