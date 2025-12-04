using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonoTask.MVC.Filters
{
    public class CustomExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var errorMessage = "An error has occured. The following error has interrupted the execution of the application: " + filterContext.Exception.Message;
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary
                {
                    {"Error", errorMessage}
                }
            };
            filterContext.ExceptionHandled = true;
        }
    }
}