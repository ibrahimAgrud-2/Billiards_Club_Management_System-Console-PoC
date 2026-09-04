using System;
using System.CodeDom;
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

         
        /// <summary>
        /// add Person to database
        /// </summary>
        /// <returns>Otomatik olarak verilen person IDsi</returns>
        public static int AddNewPerson(string firstName,  string lastName,  DateTime BirthDate,  string phone,  string address,  string email,  string imagePath)
        {
            int personID = -1;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"INSERT INTO People (FirstName,LastName,
                                                   BirthDate,Address,Phone,ImagePath, Email)
                             VALUES (@FirstName,@LastName,
                                     @BirthDate,@Address,@Phone,@ImagePath,@Email);
                             SELECT SCOPE_IDENTITY();";


                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@BirthDate", BirthDate.ToShortDateString());
                    cmd.Parameters.AddWithValue("@Phone", phone);
                

                    if (!string.IsNullOrEmpty(address))
                        cmd.Parameters.AddWithValue("@Address", address);
                    else
                        cmd.Parameters.AddWithValue("@Address", System.DBNull.Value);
                 
                    if (!string.IsNullOrEmpty(email))
                        cmd.Parameters.AddWithValue("@Email", email);
                    else
                        cmd.Parameters.AddWithValue("@Email", System.DBNull.Value);
                    
                    if (!string.IsNullOrEmpty(imagePath))
                        cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    else
                        cmd.Parameters.AddWithValue("@ImagePath", System.DBNull.Value);

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
                        Console.WriteLine("****An error occurred. Function: AddNewPerson****");
                        return -1;
                    }
                }
            }

            return personID;
        }

        public static bool UpdatePerson(int personID,string firstName, string lastName, DateTime BirthDate, string phone, string address, string email, string imagePath)
        {
            int rowsAffected = -1;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"Update  People  
                            set FirstName = @FirstName,
                                LastName = @LastName, 
                                BirthDate = @BirthDate,
                                Address = @Address,  
                                Phone = @Phone,
                                Email = @Email, 
                                ImagePath =@ImagePath
                                where PersonID = @PersonID";


                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@BirthDate", BirthDate.ToShortDateString());
                    cmd.Parameters.AddWithValue("@Phone", phone);


                    if (!string.IsNullOrEmpty(address))
                        cmd.Parameters.AddWithValue("@Address", address);
                    else
                        cmd.Parameters.AddWithValue("@Address", System.DBNull.Value);

                    if (!string.IsNullOrEmpty(email))
                        cmd.Parameters.AddWithValue("@Email", email);
                    else
                        cmd.Parameters.AddWithValue("@Email", System.DBNull.Value);

                    if (!string.IsNullOrEmpty(imagePath))
                        cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    else
                        cmd.Parameters.AddWithValue("@ImagePath", System.DBNull.Value);

                    try
                    {

                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();



                    }
                    catch (Exception)
                    {
                        //Logging will be implemented later on
                        Console.WriteLine("****An error occurred. Function: UpdatePerson****");
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }

    }
}
