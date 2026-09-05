using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CommonTools
    {

        /// <summary>
        /// Error mesajını windwos event view'a kayıt eder.
        /// </summary>
        /// <param name="ErrorType">Error türü</param>
        /// <param name="message">Erro mesajı</param>
        /// <param name="prefix">Error'un hangi katmanda olduğunu belirtmel için. Ör: BCMS.DataAccess</param>
        /// <param name="ex">Exeption mesajı</param>
        public static void LogToWindowsEventView(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            string sourceName = (prefix == "") ? "BCMS" : "BCMS." + prefix;


            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, "Application");
            }

            string detailedMessage = $"Message  : {message}";

            if (ex != null)
            {
                detailedMessage += $"\nException: {ex.GetType().Name} - {ex.Message}" +
            $"\nStackTrace:\n{ex.StackTrace}";
            }
            EventLog.WriteEntry(sourceName, detailedMessage, ErrorType);

        }

        /// <summary>
        /// Erroru Console/ekran yazdırmak için kullanılır.
        /// </summary>
        /// <param name="ErrorType">Error türü</param>
        /// <param name="message">Erro mesajı</param>
        /// <param name="prefix">Error'un hangi katmanda olduğunu belirtmel için. Ör: BCMS.DataAccess</param>
        /// <param name="ex">Exeption mesajı</param>
        public static void LogToConsole(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            string sourceName = (prefix == "") ? "BCMS" : "BCMS." + prefix;

            string detailedMessage = $"Message  : {message}";

            if (ex != null)
            {
                detailedMessage += $"\nException: {ex.GetType().Name} - {ex.Message}" +
            $"\nStackTrace:\n{ex.StackTrace}";
            }
            Console.WriteLine($"*** An Error Occurred. {detailedMessage}. ➡️ {prefix} ***");

            //if (OnErrorLogged != null)
            //{
            //    OnErrorLogged(ex.Message);
            //}

        }



}   }
