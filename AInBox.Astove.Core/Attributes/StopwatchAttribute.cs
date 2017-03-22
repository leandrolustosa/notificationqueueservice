using System;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Diagnostics;
using System.Net.Http;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StopwatchAttribute : ActionFilterAttribute
    {
        private const string StopwatchKey = "StopwatchFilter.Value";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            actionContext.Request.Properties[StopwatchKey] = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            Stopwatch stopwatch = (Stopwatch)actionExecutedContext.Request.Properties[StopwatchKey];

            if (actionExecutedContext.Response != null)
            {
                var content = actionExecutedContext.Response.Content as ObjectContent;
                if (content != null)
                {
                    var obj = content.Value;
                    if (content.ObjectType.Name.Contains("PaginatedModel"))
                    {
                        var pinfo = content.ObjectType.GetProperty("ElapsedTime");
                        pinfo.SetValue(obj, stopwatch.Elapsed.Milliseconds, null);
                    }
                    stopwatch.Stop();
                }
            }
        }        
    }
}
