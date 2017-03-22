using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AInBox.Astove.Core.Attributes;

namespace AInBox.Astove.Core.Logging
{
    public interface ILog<T>
        where T: class
    {
        [StringFormatMethodAttribute("format")]
        void Debug(string format, params object[] args);
        [StringFormatMethodAttribute("format")]
        void Info(string format, params object[] args);
        [StringFormatMethodAttribute("format")]
        void Warn(string format, params object[] args);

        [StringFormatMethodAttribute("format")]
        void Error(string format, params object[] args);
        void Error(Exception ex);

        [StringFormatMethodAttribute("format")]
        void Error(Exception ex, string format, params object[] args);

        [StringFormatMethodAttribute("format")]
        void Fatal(Exception ex, string format, params object[] args);
    }
}
