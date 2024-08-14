using Microsoft.AspNetCore.Mvc.Filters;

namespace Demo.ActionFilter
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;
      
        public ExceptionFilter(ILogger<ExceptionFilter> logger) => _logger = logger;

        public void OnException(ExceptionContext context)
        {
            _logger.LogInformation("Exception Occured");
            Console.WriteLine("Exception Occured");
        }
    }

}
