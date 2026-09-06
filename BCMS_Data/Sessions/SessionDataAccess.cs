using Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCMS_Data.Sessions
{
    public class SessionDataAccess
    {
       
        
        /// <summary>
        /// DB'deki Sessions tablosundaki tüm kayıtları alır.
        /// </summary>
        /// <returns>Sessions data table</returns>
        public static DataTable GetSessions()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "SELECT * FROM Sessions";

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
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while getting sessions",
                            "SessionDataAccess",
                            ex);
                    }
                }
            }

            return dt;
        }


        /// <summary>
        /// ID'si verilen session kaydını bulur.
        /// Bulursa tüm alanları ilgili parametrelere yükler.
        /// </summary>
        /// <returns>
        /// Session varsa true, yoksa false döner.
        /// </returns>
        public static bool Find( int sessionID,    ref int createdByStaffID,    ref int tableID,   ref DateTime sessionDate, ref DateTime sessionStartTime,     ref DateTime sessionEndTime,     ref byte sessionStatus)
        {
            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "SELECT * FROM Sessions WHERE SessionID = @SessionID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SessionID", sessionID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                createdByStaffID =
                                    Convert.ToInt32(read["CreatedByStaffID"]);

                                tableID =
                                    Convert.ToInt32(read["TableID"]);

                                sessionDate =
                                    Convert.ToDateTime(read["SessionDate"]);

                                sessionStartTime =
                                    Convert.ToDateTime(read["SessionStartTime"]);

                                sessionEndTime =
                                    Convert.ToDateTime(read["SessionEndTime"]);

                                sessionStatus =
                                    Convert.ToByte(read["SessionStatus"]);

                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while finding session",
                            "SessionDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Session ID'nin DB'de olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="sessionID">Kontrol edilecek Session ID</param>
        /// <returns>
        /// Session varsa true, yoksa false döner.
        /// </returns>
        public static bool IsSessionExists(int sessionID)
        {
            bool isFound = false;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "SELECT Found = 1 FROM Sessions WHERE SessionID = @SessionID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SessionID", sessionID);

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
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while checking for session existence",
                            "SessionDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return isFound;
        }


        /// <summary>
        /// Yeni Session kaydı ekler.
        /// </summary>
        /// <returns>
        /// DB tarafından otomatik verilen SessionID.
        /// Hata durumunda -1 döner.
        /// </returns>
        public static int AddNewSession(
            int createdByStaffID,
            int tableID,
            DateTime sessionDate,
            DateTime sessionStartTime,
            DateTime sessionEndTime,
            byte sessionStatus)
        {
            int sessionID = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"
                    INSERT INTO Sessions
                    (
                        CreatedByStaffID,
                        TableID,
                        SessionDate,
                        SessionStartTime,
                        SessionEndTime,
                        SessionStatus
                    )
                    VALUES
                    (
                        @CreatedByStaffID,
                        @TableID,
                        @SessionDate,
                        @SessionStartTime,
                        @SessionEndTime,
                        @SessionStatus
                    );

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@CreatedByStaffID",
                        createdByStaffID);

                    cmd.Parameters.AddWithValue(
                        "@TableID",
                        tableID);

                    cmd.Parameters.AddWithValue(
                        "@SessionDate",
                        sessionDate);

                    cmd.Parameters.AddWithValue(
                        "@SessionStartTime",
                        sessionStartTime);

                    cmd.Parameters.AddWithValue(
                        "@SessionEndTime",
                        sessionEndTime);

                    cmd.Parameters.AddWithValue(
                        "@SessionStatus",
                        sessionStatus);

                    try
                    {
                        connection.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null &&
                            int.TryParse(
                                result.ToString(),
                                out int insertedID))
                        {
                            sessionID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while adding session",
                            "SessionDataAccess",
                            ex);

                        return -1;
                    }
                }
            }

            return sessionID;
        }


        /// <summary>
        /// ID'si verilen Session kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true, değilse false döner.
        /// </returns>
        public static bool UpdateSession(
            int sessionID,
            int createdByStaffID,
            int tableID,
            DateTime sessionDate,
            DateTime sessionStartTime,
            DateTime sessionEndTime,
            byte sessionStatus)
        {
            int rowsAffected = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"
                    UPDATE Sessions
                    SET
                        CreatedByStaffID = @CreatedByStaffID,
                        TableID = @TableID,
                        SessionDate = @SessionDate,
                        SessionStartTime = @SessionStartTime,
                        SessionEndTime = @SessionEndTime,
                        SessionStatus = @SessionStatus
                    WHERE SessionID = @SessionID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@SessionID",
                        sessionID);

                    cmd.Parameters.AddWithValue(
                        "@CreatedByStaffID",
                        createdByStaffID);

                    cmd.Parameters.AddWithValue(
                        "@TableID",
                        tableID);

                    cmd.Parameters.AddWithValue(
                        "@SessionDate",
                        sessionDate);

                    cmd.Parameters.AddWithValue(
                        "@SessionStartTime",
                        sessionStartTime);

                    cmd.Parameters.AddWithValue(
                        "@SessionEndTime",
                        sessionEndTime);

                    cmd.Parameters.AddWithValue(
                        "@SessionStatus",
                        sessionStatus);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while updating session",
                            "SessionDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }


        /// <summary>
        /// ID'si verilen Session kaydını siler.
        /// </summary>
        /// <param name="sessionID">Silinecek Session ID</param>
        /// <returns>
        /// Delete işlemi başarılıysa true, değilse false döner.
        /// </returns>
        public static bool DeleteSession(int sessionID)
        {
            int rowsAffected = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "DELETE Sessions WHERE SessionID = @SessionID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@SessionID",
                        sessionID);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while deleting session",
                            "SessionDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
    }
}