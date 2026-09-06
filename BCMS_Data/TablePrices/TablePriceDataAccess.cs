using Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCMS_Data
{
    public class TablePriceDataAccess
    {
        /// <summary>
        /// DB'deki Table TablePrice tablosundaki tüm kayıtları alır.
        /// </summary>
        /// <returns>TablePrice data table</returns>
        public static DataTable GetPrices()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection =new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "SELECT * FROM TablePrice";

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
                            "An Error occurred while getting prices",
                            "TablePriceDataAccess",
                            ex);
                    }
                }
            }

            return dt;
        }


        /// <summary>
        /// ID'si verilen TablePrice kaydını bulur.
        /// Bulursa tüm alanları ilgili parametrelere yükler.
        /// </summary>
        /// <returns>
        /// TablePrice kaydı varsa true, yoksa false döner.
        /// </returns>
        public static bool Find(  int priceID,  ref int createdByUserID,  ref string description,ref decimal pricePerHour)
        {
            using (SqlConnection connection =new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "SELECT * FROM TablePrice WHERE PriceID = @PriceID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PriceID", priceID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                createdByUserID = Convert.ToInt32(read["CreatedByUserID"]);

                                description = read["Description"]?.ToString() ?? null;

                                pricePerHour = Convert.ToDecimal(read["pricePerHour"]);

                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while finding price",
                            "TablePriceDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// ID'si verilen TablePrice kaydının DB'de olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="priceID">Kontrol edilecek TablePrice ID</param>
        /// <returns>
        /// Kayıt varsa true, yoksa false döner.
        /// </returns>
        public static bool IsPriceExists(int priceID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query ="SELECT Found = 1 FROM TablePrice WHERE PriceID = @PriceID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PriceID", priceID);

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
                            "An Error occurred while checking for price existence",
                            "TablePriceDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return isFound;
        }


        /// <summary>
        /// Yeni TablePrice kaydı ekler.
        /// </summary>
        /// <returns>
        /// DB tarafından otomatik verilen PriceID.
        /// Hata durumunda -1 döner.
        /// </returns>
        public static int AddNewPrice(    int createdByUserID, string description, decimal pricePerHour)
        {
            int priceID = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"
                    INSERT INTO TablePrice
                    (
                        CreatedByUserID,
                        Description,
                        pricePerHour
                    )
                    VALUES
                    (
                        @CreatedByUserID,
                        @Description,
                        @pricePerHour
                    );

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@CreatedByUserID",
                        createdByUserID);

                    if (!string.IsNullOrEmpty(description))
                        cmd.Parameters.AddWithValue("@Description", description);
                    else
                        cmd.Parameters.AddWithValue(
                            "@Description",
                            System.DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@pricePerHour",
                        pricePerHour);

                    try
                    {
                        connection.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null &&
                            int.TryParse(result.ToString(), out int insertedID))
                        {
                            priceID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while adding price",
                            "TablePriceDataAccess",
                            ex);

                        return -1;
                    }
                }
            }

            return priceID;
        }


        /// <summary>
        /// ID'si verilen TablePrice kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true, değilse false.
        /// </returns>
        public static bool UpdatePrice(  int priceID,int createdByUserID,string description, decimal pricePerHour)
        {
            int rowsAffected = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"
                    UPDATE TablePrice
                    SET
                        CreatedByUserID = @CreatedByUserID,
                        Description = @Description,
                        pricePerHour = @pricePerHour
                    WHERE PriceID = @PriceID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PriceID", priceID);

                    cmd.Parameters.AddWithValue(
                        "@CreatedByUserID",
                        createdByUserID);

                    if (!string.IsNullOrEmpty(description))
                        cmd.Parameters.AddWithValue("@Description", description);
                    else
                        cmd.Parameters.AddWithValue(
                            "@Description",
                            System.DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@pricePerHour",
                        pricePerHour);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while updating price",
                            "TablePriceDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }


        /// <summary>
        /// ID'si verilen TablePrice kaydını siler.
        /// </summary>
        /// <param name="priceID">Silinecek TablePrice ID</param>
        /// <returns>
        /// Delete işlemi başarılıysa true, değilse false.
        /// </returns>
        public static bool DeletePrice(int priceID)
        {
            int rowsAffected = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "DELETE TablePrice WHERE PriceID = @PriceID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PriceID", priceID);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while deleting price",
                            "TablePriceDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
    }
}