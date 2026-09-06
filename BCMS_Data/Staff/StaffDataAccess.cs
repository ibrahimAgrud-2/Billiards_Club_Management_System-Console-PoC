
using Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCMS_Data
{
    public class StaffDataAccess
    {

        /// <summary>
        /// DB'deki Staff tablosundaki tüm tabloyu olduğu gibi alır
        /// </summary>
        /// <returns>Staff data table</returns>
        public static DataTable GetStaff()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "select * from Staff ";

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
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while getting staff", "StaffDataAccess", ex);
                    }
                }
            }

            return dt;
        }


        /// <summary>
        /// Eğer personeli bulursa her bir parametreye personelin verisini yükler.
        /// </summary>
        /// <returns>Personel varsa true, yoksa false</returns>
        public static bool Find(int staffID, ref int personID, ref decimal salary, ref DateTime hireDate, ref int createdByUserID, ref DateTime? separationDate, ref bool stillWorking)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "select * from Staff where StaffID=@StaffID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StaffID", staffID);

                    try
                    {
                        connection.Open();                                                                                                                                          

                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                personID = Convert.ToInt32(read["PersonID"]);
                                salary = Convert.ToDecimal(read["Salary"]);
                                hireDate = Convert.ToDateTime(read["HireDate"]);
                                createdByUserID = Convert.ToInt32(read["CreatedByUserID"]);
                                if(read["SeparationDate"] == DBNull.Value)
                                {
                                    separationDate = null;
                                }
                                else
                                {
                                    separationDate = Convert.ToDateTime(read["SeparationDate"]);
                                }
                                    stillWorking = Convert.ToBoolean(read["StillWorking"]);

                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while finding staff", "StaffDataAccess", ex);
                        return false;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Personelin olup olmadığını kontrol eder
        /// </summary>
        /// <param name="staffID"></param>
        /// <returns>Eğer personel DB'de varsa true, yoksa false döner</returns>
        public static bool IsStaffExists(int staffID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "SELECT Found=1 FROM Staff WHERE StaffID = @StaffID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StaffID", staffID);

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
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while checking for staff existence", "StaffDataAccess", ex);

                        return false;
                    }
                }
            }

            return isFound;
        }


        /// <summary>
        /// Add Staff to database
        /// </summary>
        /// <returns>Otomatik olarak verilen staff IDsi</returns>
        public static int AddNewStaff(int personID, decimal salary, DateTime hireDate, int createdByUserID, DateTime? separationDate, bool stillWorking)
        {
            int staffID = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"INSERT INTO Staff (PersonID, Salary, HireDate, CreatedByUserID, SeparationDate, StillWorking)
                                 VALUES (@PersonID, @Salary, @HireDate, @CreatedByUserID, @SeparationDate, @StillWorking);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.Parameters.AddWithValue("@Salary", salary);
                    cmd.Parameters.AddWithValue("@HireDate", hireDate);
                    cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                    if (separationDate.HasValue)
                        cmd.Parameters.AddWithValue("@SeparationDate", separationDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@SeparationDate", System.DBNull.Value);

                    cmd.Parameters.AddWithValue("@StillWorking", stillWorking);

                    try
                    {
                        connection.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            staffID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while adding staff", "StaffDataAccess", ex);

                        return -1;
                    }
                }
            }

            return staffID;
        }


        /// <summary>
        /// ID ile staff tüm alanları günceller.
        /// </summary>
        /// <returns>Eğer update yaparken sorun çıkmazsa true</returns>
        public static bool UpdateStaff(int staffID, int personID, decimal salary, DateTime hireDate, int createdByUserID, DateTime? separationDate, bool stillWorking)
        {
            int rowsAffected = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"Update Staff
                                 set PersonID = @PersonID,
                                     Salary = @Salary,
                                     HireDate = @HireDate,
                                     CreatedByUserID = @CreatedByUserID,
                                     SeparationDate = @SeparationDate,
                                     StillWorking = @StillWorking
                                 where StaffID = @StaffID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StaffID", staffID);
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.Parameters.AddWithValue("@Salary", salary);
                    cmd.Parameters.AddWithValue("@HireDate", hireDate);
                    cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                    if (separationDate.HasValue)
                        cmd.Parameters.AddWithValue("@SeparationDate", separationDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@SeparationDate", System.DBNull.Value);

                    cmd.Parameters.AddWithValue("@StillWorking", stillWorking);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while updating staff", "StaffDataAccess", ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }


        /// <summary>
        /// ID'sini verdiğin staff'ı siler.
        /// </summary>
        /// <param name="staffID">Silinecek staff'ın ID'si</param>
        /// <returns></returns>
        public static bool DeleteStaff(int staffID)
        {
            int rowsAffected = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"delete Staff where StaffID=@StaffID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StaffID", staffID);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while deleting staff", "StaffDataAccess", ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
    }
}

