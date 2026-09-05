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
        //Log fonksiyonları
        private static void LogToWindowsEventView(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
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
    
        private static void LogToConsole(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            string sourceName = (prefix == "") ? "BCMS" : "BCMS." + prefix;

            string detailedMessage = $"Message  : {message}";

            if (ex != null)
            {
                detailedMessage += $"\nException: {ex.GetType().Name} - {ex.Message}" +
            $"\nStackTrace:\n{ex.StackTrace}";
            }
            Console.WriteLine(" *** An Error Occurred. " + detailedMessage + " ***");

            //if (OnErrorLogged != null)
            //{
            //    OnErrorLogged(ex.Message);
            //}

        }

        private static void LogToDatabase(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            string sourceName = (prefix == "") ? "BCMS" : "BCMS." + prefix;

            string detailedMessage = $"Message  : {message}";

            if (ex != null)
            {
                detailedMessage += $"\nException: {ex.GetType().Name} - {ex.Message}" +
            $"\nStackTrace:\n{ex.StackTrace}";
            }
            Console.WriteLine(" *** An Error Occurred. " + detailedMessage + " ***");



            int personID = -1;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"INSERT INTO Logs (@LogMessage,@LogDate,@LogDetails);
                             SELECT SCOPE_IDENTITY();";


                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LogMessage", message);
                    cmd.Parameters.AddWithValue("@LogDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LogDetails", detailedMessage);



                    try
                    {
                        connection.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            personID = insertedID;
                        }
                    }
                    catch (Exception)
                    {
                        //Logging will be implemented later on
                        Console.WriteLine("****An error occurred. Function: LogToDatabase****");

                    }
                }
            }



            //if (OnErrorLogged != null)
            //{
            //    OnErrorLogged(ex.Message);
            //}

        }

         public static Logger log = new Logger(LogToConsole);

    }
}
