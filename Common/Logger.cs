using System;
using System.Diagnostics;


namespace Common
{
    public static class Logger
    {

        public delegate void LogAction(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null);
        
        private static LogAction _LogAction;
       

        //pL'den çağırılacak fonksiyon.
        public static void SetLogAction(LogAction logAction)
        {
            _LogAction = logAction;
        }

        //Diğer herhangi bir yerden çağırılacak fonksiyon.
        public static void Log(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            //hangi fonk (toDatabase, toConsole, toEventView ) abone ise onu sadece çağıracak
            _LogAction?.Invoke(ErrorType, message, prefix, ex);
        }
    }
}
