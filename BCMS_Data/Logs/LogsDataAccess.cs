using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace BCMS_Data.Logs
{
    public class LogsDataAccess
    {
        /// <summary>
        /// DB'de tüm log kayıtlarını alır
        /// </summary>
        /// <returns>Logs data table</returns>
        public static DataTable GetLogs()
        {

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
           
            string sqlQuery = @"select * from Logs";

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            try
            {
                connection.Open();
                SqlDataReader read = cmd.ExecuteReader();

                if (read.HasRows)
                {
                    dt.Load(read);
                }
                read.Close();
            }
            catch (Exception ex)
            {
                //Logging will be implemented later on
            }
            finally
            {
                connection.Close();
            }

            return dt;


        }
        

    }
}
