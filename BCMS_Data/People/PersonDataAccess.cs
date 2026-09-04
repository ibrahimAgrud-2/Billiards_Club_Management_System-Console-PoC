using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Security.Policy;


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
                //Logging will be implemented later on
            }
            finally
            {
                connection.Close();
            }

            return dt;

        }


        public static bool Find(int personID, ref string firstName, ref string lastName, ref DateTime dateOfBirth, ref string phone, ref string address, ref string email, ref string imagePath)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "select * from People where PersonID=@ID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", personID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                firstName = read["FirstName"].ToString();
                                lastName = read["LastName"].ToString();
                                dateOfBirth = Convert.ToDateTime(read["BirthDate"]);
                                phone = read["Phone"].ToString();

                                //nullable values. Eğer değişken null ise null dönsün. Bu sayede programın diğer kısımlarında
                                //nullOrEmpty kontrolü yapabiliriz. default değer verseydik N/A gibi bu sefer değilken dolu olurdu.
                                address = read["Address"]?.ToString()??null;
                                email = read["Email"]?.ToString() ?? null;
                                imagePath = read["ImagePath"]?.ToString() ?? null;

                                return true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Logging will be implemented later on
                        Console.WriteLine("****An error occurred while reading finding person****");
                        return false;
                    }
                }
            }
            return false;
        }

        public static bool IsPersonExists(int personID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "SELECT Found=1 FROM People WHERE PersonID = @PersonID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        isFound = reader.HasRows;

                    }
                    catch (Exception)
                    {
                        //Logging will be implemented later on
                        Console.WriteLine("****An error occurred. Funtion: IsPersonExists****");
                        return false;
                    }
                }
            }
            return false;
        }

    }
}
