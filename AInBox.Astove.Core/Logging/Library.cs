using AInBox.Astove.Core.Extensions;
using System;
using System.IO;

namespace AInBox.Astove.Core.Logging
{
    public class Library
    {
        private static string emailQueueLogFile = "EmailQueueLogFile.txt";
        public void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\", emailQueueLogFile), true);
                sw.WriteLine(string.Format("{0}: {1}: {2}", DateTime.Now.ToString(), ex.Source.Trim(), ex.GetExceptionMessageWithStackTrace()));
            }
            catch
            {
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }
        }

        public void WriteErrorLog(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\", emailQueueLogFile), true);
                sw.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString(), message));
            }
            catch
            {
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }
        }
    }
}
