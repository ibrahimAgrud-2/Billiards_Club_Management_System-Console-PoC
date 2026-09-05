using Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Policy;




namespace BCMS_Data.People
{
    public class PersonDataAccess
    {

        /// <summary>
        /// DB'de people tablosundaki tüm tabloyu olduğu gibi alır
        /// </summary>
        /// <returns>People data table</returns>
        public static DataTable GetPeople()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "select * from People ";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                  

                    try
                    {
                        connection.Open();
                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.HasRows)
                            {
                                dt.Load(read);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while finding person", "PersonDataAccess", ex);
                        
                    }
                }
            }
            return dt;
        }


        /// <summary>
        /// Eğer kişiyi bulursa her bir parametreye kişini verisini yükler.
        /// </summary>
        /// <returns>Kişi varsa true, yoksa false</returns>
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
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while finding person", "PersonDataAccess", ex);
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// kişisini olup olmadığını kontrol eder
        /// </summary>
        /// <param name="personID"></param>
        /// <returns>Eğer kişi DB'de varsa true, yoksa false döner</returns>
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
                       
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            isFound = reader.HasRows;
                        }
                          

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while checking for person existence", "PersonDataAccess", ex);

                        return false;
                    }
                }
            }
            return isFound;
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
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while adding person", "PersonDataAccess", ex);

                        return -1;
                    }
                }
            }

            return personID;
        }

        /// <summary>
        /// ID ile person tüm alanları günceller.
        /// </summary>
        /// <returns>Eğer update yaparken sorun çıkmazsa true</returns>
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
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while updating person", "PersonDataAccess", ex);
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }

        /// <summary>
        /// ID'sini verdiğini kişiyi siler.
        /// </summary>
        /// <param name="personID">Silinecek kişinin ID'si</param>
        /// <returns></returns>
        public static bool DeletePerson(int personID)
        {
            int rowsAffected = -1;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"delete People where personID=@PersonID";


                using (SqlCommand cmd = new SqlCommand(query, connection))
                {

                    cmd.Parameters.AddWithValue(@"PersonID",personID);


                    try
                    {

                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();



                    }
                    catch (Exception ex)
                    {

                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning,"An Error occurred while deleting person","PersonDataAccess",ex);
                     

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);

        }


    }
}
