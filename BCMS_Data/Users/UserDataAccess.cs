using Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCMS_Data.Users
{
    public class UserDataAccess
    {

        /// <summary>
        /// DB'deki Users tablosundaki tüm tabloyu olduğu gibi alır
        /// </summary>
        /// <returns>Users data table</returns>
        public static DataTable GetUsers()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "select * from Users ";

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
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while getting users", "UserDataAccess", ex);
                    }
                }
            }

            return dt;
        }


        /// <summary>
        /// Eğer kullanıcıyı bulursa her bir parametreye kullanıcının verisini yükler.
        /// </summary>
        /// <returns>Kullanıcı varsa true, yoksa false</returns>
        public static bool Find(int ID, ref int personID, ref string userName, ref string password, ref bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "select * from Users where ID=@ID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                personID = Convert.ToInt32(read["PersonID"]);
                                userName = read["UserName"].ToString();
                                password = read["Password"].ToString();
                                isActive = Convert.ToBoolean(read["IsActive"]);

                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while finding user", "UserDataAccess", ex);
                        return false;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Kullanıcının olup olmadığını kontrol eder
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>Eğer kullanıcı DB'de varsa true, yoksa false döner</returns>
        public static bool IsUserExists(int ID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "SELECT Found=1 FROM Users WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);

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
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while checking for user existence", "UserDataAccess", ex);

                        return false;
                    }
                }
            }

            return isFound;
        }


        /// <summary>
        /// Add User to database
        /// </summary>
        /// <returns>Otomatik olarak verilen user IDsi</returns>
        public static int AddNewUser(int personID, string userName, string password, bool isActive)
        {
            int ID = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"INSERT INTO Users (PersonID,UserName,Password,IsActive)
                                 VALUES (@PersonID,@UserName,@Password,@IsActive);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);

                    try
                    {
                        connection.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            ID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while adding user", "UserDataAccess", ex);

                        return -1;
                    }
                }
            }

            return ID;
        }


        /// <summary>
        /// ID'si verilen user'ın tüm alanlarını günceller.
        /// </summary>
        /// <returns>Eğer update yaparken sorun çıkmazsa true</returns>
        public static bool UpdateUser(int ID, int personID, string userName, string password, bool isActive)
        {
            int rowsAffected = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"Update Users
                                 set PersonID = @PersonID,
                                     UserName = @UserName,
                                     Password = @Password,
                                     IsActive = @IsActive
                                 where ID = @ID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while updating user", "UserDataAccess", ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }


        /// <summary>
        /// ID'sini verdiğin kullanıcıyı siler.
        /// </summary>
        /// <param name="ID">Silinecek kullanıcının ID'si</param>
        /// <returns></returns>
        public static bool DeleteUser(int ID)
        {
            int rowsAffected = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"delete Users where ID=@ID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while deleting user", "UserDataAccess", ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
    }
}

