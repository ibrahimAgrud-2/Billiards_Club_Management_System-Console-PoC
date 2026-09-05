using BCMS_Data.LogMessages;
using System;
using System.Diagnostics;


namespace BCMS_Business.LogMessages
{
    public class Logs
    {
        public static void LogToDatabase(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            DBLog.LogToDatabase(ErrorType,message,prefix,ex);
        }
    }
}

