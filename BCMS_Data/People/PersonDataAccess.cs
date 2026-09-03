using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;


namespace BCMS_Data.People
{
    public class PersonDataAccess
    {
        public static DataTable GetPeople()
        {
            DataTable dt = new DataTable();



            SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            //Query'deki field sırası önemli. Çünkü bu sırayla dgv'de gözükecek.
            string sqlQuery = @"select * from People";


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
                //log message to event viewer
            }
            finally
            {
                connection.Close();
            }



            return dt;

        }

       

    }
}
