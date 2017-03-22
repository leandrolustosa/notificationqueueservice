using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetExceptionMessage(this Exception ex)
        {
            return (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
        }

        public static string GetExceptionMessageWithStackTrace(this Exception ex)
        {
            var formatStr = "Message: {0} \n\n Stack Trace: {1}";
            return (ex.InnerException != null) ? string.Format(formatStr, ex.InnerException.Message, ex.InnerException.StackTrace) : string.Format(formatStr, ex.Message, ex.StackTrace);
        }
    }
}
