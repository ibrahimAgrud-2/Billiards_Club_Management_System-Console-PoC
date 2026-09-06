using Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCMS_Data.Tables
{
    public class TableDataAccess
    {
        /// <summary>
        /// DB'deki Tables tablosundaki tüm kayıtları alır.
        /// </summary>
        /// <returns>Tables data table</returns>
        public static DataTable GetTables()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "SELECT * FROM Tables";

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
                            "An Error occurred while getting tables",
                            "TableDataAccess",
                            ex);
                    }
                }
            }

            return dt;
        }


        /// <summary>
        /// ID'si verilen table kaydını bulur.
        /// Bulursa tüm alanları ilgili parametrelere yükler.
        /// </summary>
        /// <returns>
        /// Table kaydı varsa true, yoksa false.
        /// </returns>
        public static bool Find( int tableID,  ref int priceID,   ref byte tableType,ref byte tableStatus)
        {
            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "SELECT * FROM Tables WHERE TableID = @TableID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TableID", tableID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                priceID = Convert.ToInt32(read["PriceID"]);

                                tableType = (byte)read["TableType"];

                                tableStatus = (byte)read["TableStatus"];

                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while finding table",
                            "TableDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// ID'si verilen table kaydının DB'de olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="tableID">Kontrol edilecek Table ID</param>
        /// <returns>
        /// Kayıt varsa true, yoksa false.
        /// </returns>
        public static bool IsTableExists(int tableID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "SELECT Found = 1 FROM Tables WHERE TableID = @TableID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TableID", tableID);

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
                            "An Error occurred while checking for table existence",
                            "TableDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return isFound;
        }


        /// <summary>
        /// Yeni table kaydı ekler.
        /// </summary>
        /// <returns>
        /// DB tarafından otomatik verilen TableID.
        /// Hata durumunda -1 döner.
        /// </returns>
        public static int AddNewTable(
            int priceID,
            byte tableType,
            byte tableStatus)
        {
            int tableID = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"
                    INSERT INTO Tables
                    (
                        PriceID,
                        TableType,
                        TableStatus
                    )
                    VALUES
                    (
                        @PriceID,
                        @TableType,
                        @TableStatus
                    );

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PriceID", priceID);
                    cmd.Parameters.AddWithValue("@TableType", tableType);
                    cmd.Parameters.AddWithValue("@TableStatus", tableStatus);


                    try
                    {
                        connection.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null &&
                            int.TryParse(result.ToString(), out int insertedID))
                        {
                            tableID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while adding table",
                            "TableDataAccess",
                            ex);

                        return -1;
                    }
                }
            }

            return tableID;
        }


        /// <summary>
        /// ID'si verilen table kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true, değilse false.
        /// </returns>
        public static bool UpdateTable(
            int tableID,
            int priceID,
            byte tableType,
            byte tableStatus)
        {
            int rowsAffected = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"
                    UPDATE Tables
                    SET
                        PriceID = @PriceID,
                        TableType = @TableType,
                        TableStatus = @TableStatus
                    WHERE TableID = @TableID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TableID", tableID);
                    cmd.Parameters.AddWithValue("@PriceID", priceID);

                    cmd.Parameters.AddWithValue("@TableType", tableType);
                    cmd.Parameters.AddWithValue("@TableStatus", tableStatus);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while updating table",
                            "TableDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }


        /// <summary>
        /// ID'si verilen table kaydını siler.
        /// </summary>
        /// <param name="tableID">Silinecek Table ID</param>
        /// <returns>
        /// Delete işlemi başarılıysa true, değilse false.
        /// </returns>
        public static bool DeleteTable(int tableID)
        {
            int rowsAffected = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "DELETE Tables WHERE TableID = @TableID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TableID", tableID);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while deleting table",
                            "TableDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
    }
}