using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMS_Business.Logs
{
    public class Logs
    {
        public static void LogToDatabase(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            BCMS_Data.Logs.DBLog.LogToDatabase(ErrorType, message, prefix, ex);
        }
    }
}

