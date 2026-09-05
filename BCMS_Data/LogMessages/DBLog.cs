using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMS_Data.LogMessages
{
    public class DBLog
    {


        /// <summary>
        /// Erroru DB'e yazdırmak için kullanılır. Bu fonksiyon DB'e eriştiği için DL katmanda olmalı.
        /// </summary>
        /// <param name="ErrorType">Error türü</param>
        /// <param name="message">Erro mesajı</param>
        /// <param name="prefix">Error'un hangi katmanda olduğunu belirtmel için. Ör: BCMS.DataAccess</param>
        /// <param name="ex">Exeption mesajı</param>
        public static void LogToDatabase(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            string sourceName = (prefix == "") ? "BCMS" : "BCMS." + prefix;

            string detailedMessage = $"Message  : {message}";

            if (ex != null)
            {
                detailedMessage += $"\nException: {ex.GetType().Name} - {ex.Message}" +
            $"\nStackTrace:\n{ex.StackTrace}";
            }
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"INSERT INTO Logs (LogMessage,LogDate,
                                                   LogDetails)
                             VALUES (@LogMessage,@LogDate,
                                     @LogDetails);
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
                    }
                    catch (Exception)
                    {

                        Console.WriteLine("****An error occurred. Function: LogToDatabase****");

                    }
                }
            }

        }

    }
}
