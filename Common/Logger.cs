using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Logger
    {
        public enum EventLogID
        {
            // DL connection errorlar için
            DL_ConnectionError = 1001,


            // BL için - 2000'ler
            BLL_ValidationError = 2001,
            BLL_BusinessRuleViolation = 2002,
            BLL_UnauthorizedAccess = 2003,

            // UI - 3000'ler
            UI_UnhandledException = 3001,
            UI_FormLoadError = 3002,

            // Genel
            General_UnknownError = 9999
        }

        public static event Action<string> OnErrorLogged;

        public static void LogMessage(EventLogEntryType ErrorType, EventLogID EventID,string message , string prefix = "",Exception ex=null)
        {
        string sourceName = (prefix == "") ? "DVLD" : "DVLD." + prefix;


            

            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, "Application");
            }

            string detailedMessage= $"Message  : {message}";

            if(ex!=null)
            {
                detailedMessage += $"\nException: {ex.GetType().Name} - {ex.Message}" +
            $"\nStackTrace:\n{ex.StackTrace}";
            }
            EventLog.WriteEntry(sourceName, detailedMessage, ErrorType, (int)EventID);
           if(OnErrorLogged!=null)
            {
                OnErrorLogged(ex.Message);
            }
           


        }
    }
}
