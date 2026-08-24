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

        public static bool Find(int personID, ref string firstName, ref string lastName, ref DateTime? dateOfBirth, ref string phone, ref string address, ref string email,ref string imagePath)
        {
            
            SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

            string query = "select * from People where ID=@ID";

            SqlCommand cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@ID", personID);

            try
            {
                connection.Open();
                SqlDataReader read = cmd.ExecuteReader();

                if (read.Read())
                {

                    personID = Convert.ToInt32(read["ID"]);
                    firstName = read["firstName"].ToString();
                    lastName = read["lastName"].ToString();
                    dateOfBirth = Convert.ToDateTime(read["dateOfBirth"]);
                    phone = read["phone"].ToString();

                    address = read["address"]?.ToString();
                    email = read["Email"]?.ToString();
                    imagePath = read["ImagePath"]?.ToString();







                    read.Close();

                    return true;
                }

            }
            catch (Exception ex)
            {
                //Log message to view logger
                return false;
            }
            finally
            {
                connection.Close();

            }

            return false;

        }




    }
}
