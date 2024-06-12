using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Demo.ActionFilter
{
    public class LogActionFilter : ActionFilterAttribute 
    {
        private readonly ILogger<LogActionFilter> _logger;
        public LogActionFilter(ILogger<LogActionFilter> logger) =>
            _logger = logger;

        // ActionFilter
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Log("OnActionExecuting", filterContext.RouteData);
            _logger.LogInformation(
                $"- {nameof(LogActionFilter)}.{nameof(OnActionExecuting)}");
            Console.WriteLine("onActionExecuting");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Log("OnActionExecuted", filterContext.RouteData);
            Console.WriteLine("OnActionExecuted");
        }

        // ResultFilter
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Log("OnResultExecuting", filterContext.RouteData);
            Console.WriteLine("OnResultExecuting");
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Log("OnResultExecuted", filterContext.RouteData);
            Console.WriteLine("OnResultExecuting");
        }


        private void Log(string methodName, RouteData routeData)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            var message = String.Format("{0} controller:{1} action:{2}", methodName, controllerName, actionName);
            _logger.LogInformation($"- {message}");
            Console.WriteLine(message, "Action Filter Log");
        }

     
    }
}
