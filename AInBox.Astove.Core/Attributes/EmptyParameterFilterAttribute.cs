using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EmptyParameterFilterAttribute : ActionFilterAttribute
    {
        public string ParameterName { get; private set; }

        public EmptyParameterFilterAttribute(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            ParameterName = parameterName;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            object parameterValue;
            if (actionContext.ActionArguments.TryGetValue(ParameterName, out parameterValue))
            {
                if (parameterValue == null)
                {
                    actionContext.ModelState.AddModelError(ParameterName, FormatErrorMessage(ParameterName));
                    
                    var errors = new List<string>();
                    foreach (var state in actionContext.ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }

                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, errors);
                    //actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                }
            }
        }

        private string FormatErrorMessage(string parameterName)
        {
            return string.Format("O {0} não pode ser nulo.", parameterName);
        }
    }
}
