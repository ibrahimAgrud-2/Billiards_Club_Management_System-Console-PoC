using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Common.Logger;

namespace Common
{
    public class Logger
    {


        //Bir değişken türü tanımladık. Delegate=değişken türü
        //Bu değişkten türü normal sayı veya int yerine fonskiyon tutabilen bir tür.
        public delegate void LogAction(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null);

        //tanımladığımız değiştken türünden bir değişken tanımlayalım. 
        //Yukarda sadece değişken türü tanınladık. Sanki string'i tanımlamışız gibi
        //şimdi de string name gibi kullanabilmek için yani o türden değerler tutabilmek için bir değiken tanımlamalıyız (Kutu)
        private static LogAction _LogAction;
       
        /// <summary>
        /// Bu const sınıftan bir obje oluşturduğumuzda illa ilgili fonskiyonu bizden parameter olarak ister
        /// Yani Log işlemini hangi fonk ile yapacaksak onu önceden bilmesi gerekecek
        /// </summary>
        /// <param name="logAction"></param>
        public Logger(LogAction logAction)
        {
            _LogAction = logAction;
        }

        /// <summary>
        /// Bu fonksiyon aracı fonksiyon. İşi hangi fonk abone ise onu delegate ile çağıracak.
        /// </summary>
        public static void Log(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            //hangi fonk (toDatabase, toConsole, toEventView ) abone ise onu sadece çağıracak
            _LogAction?.Invoke(ErrorType, message, prefix, ex);
        }


        //Log fonksiyonları
        public static void LogToWindowsEventView(EventLogEntryType ErrorType,string message , string prefix = "",Exception ex=null)
        {
        string sourceName = (prefix == "") ? "BCMS" : "BCMS." + prefix;


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
            EventLog.WriteEntry(sourceName, detailedMessage, ErrorType);
        
        }
        public static void LogToConsole(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
        {
            string sourceName = (prefix == "") ? "BCMS" : "BCMS." + prefix;
   
                string detailedMessage = $"Message  : {message}";

                if (ex != null)
                {
                    detailedMessage += $"\nException: {ex.GetType().Name} - {ex.Message}" +
                $"\nStackTrace:\n{ex.StackTrace}";
                }
               Console.WriteLine(" *** An Error Occurred. " +detailedMessage +" ***");
            
            //if (OnErrorLogged != null)
            //{
            //    OnErrorLogged(ex.Message);
            //}

        }

        public static void LogToDatabase(EventLogEntryType ErrorType, string message, string prefix = "", Exception ex = null)
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

    }
}
