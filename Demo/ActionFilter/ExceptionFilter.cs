using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;

namespace Demo.ActionFilter
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger) =>
            _logger = logger;
        public void OnException(ExceptionContext context)
        {
            //context.Result = new ContentResult
            //{
            //    Content = context.Exception.ToString()
            //};
            _logger.LogInformation("Exception Occured");
            Console.WriteLine("Exception Occured");
        }
    }

}
