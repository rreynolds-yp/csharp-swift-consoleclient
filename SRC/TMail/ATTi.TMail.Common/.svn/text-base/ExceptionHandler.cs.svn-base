using System;

namespace ATTi.TMail.Common
{
    public static class ExceptionHandler
    {

        public static string ExceptionToString(this Exception ex)
        {
            return string.Format("{0} : {1}\r\n{2}", ex.GetType().FullName, DateTime.UtcNow,
                                       ExceptionString(ex));
        }

        private static string ExceptionString(Exception ex)
        {
            return string.Format("{0}\r\n\r\n{1}\r\n\r\n{2}", ex.Message, ex.StackTrace,
                                 ex.InnerException == null
                                     ? string.Empty
                                     : string.Format("[[\r\n{0}\r\n]]", ExceptionString(ex.InnerException)));
        }
    }
}