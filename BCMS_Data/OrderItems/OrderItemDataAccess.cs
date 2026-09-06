using Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCMS_Data.OrderItems
{
    public class OrderItemDataAccess
    {
        /// <summary>
        /// DB'deki OrderItems tablosundaki tüm kayıtları alır.
        /// </summary>
        /// <returns>OrderItems data table</returns>
        public static DataTable GetOrderItems()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "SELECT * FROM OrderItems";

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
                            "An Error occurred while getting order items",
                            "OrderItemDataAccess",
                            ex);
                    }
                }
            }

            return dt;
        }


        /// <summary>
        /// ID'si verilen OrderItem kaydını bulur.
        /// Bulursa tüm alanları ilgili parametrelere yükler.
        /// </summary>
        /// <returns>
        /// OrderItem varsa true, yoksa false.
        /// </returns>
        public static bool Find(
            int itemID,
            ref int sessionID,
            ref int productID,
            ref short quantity,
            ref short productPice)
        {
            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "SELECT * FROM OrderItems WHERE ItemID = @ItemID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                sessionID =
                                    Convert.ToInt32(read["SessionID"]);

                                productID =
                                    Convert.ToInt32(read["ProductID"]);

                                quantity =
                                    Convert.ToInt16(read["Quantity"]);

                                productPice =
                                    Convert.ToInt16(read["ProductPice"]);

                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while finding order item",
                            "OrderItemDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Item ID'nin DB'de olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="itemID">Kontrol edilecek Item ID</param>
        /// <returns>
        /// Kayıt varsa true, yoksa false.
        /// </returns>
        public static bool IsOrderItemExists(int itemID)
        {
            bool isFound = false;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "SELECT Found = 1 FROM OrderItems WHERE ItemID = @ItemID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemID);

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
                            "An Error occurred while checking for order item existence",
                            "OrderItemDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return isFound;
        }


        /// <summary>
        /// Yeni OrderItem kaydı ekler.
        /// </summary>
        /// <returns>
        /// DB tarafından otomatik verilen ItemID.
        /// Hata durumunda -1 döner.
        /// </returns>
        public static int AddNewOrderItem(
            int sessionID,
            int productID,
            short quantity,
            short productPice)
        {
            int itemID = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"
                    INSERT INTO OrderItems
                    (
                        SessionID,
                        ProductID,
                        Quantity,
                        ProductPice
                    )
                    VALUES
                    (
                        @SessionID,
                        @ProductID,
                        @Quantity,
                        @ProductPice
                    );

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@SessionID",
                        sessionID);

                    cmd.Parameters.AddWithValue(
                        "@ProductID",
                        productID);

                    cmd.Parameters.AddWithValue(
                        "@Quantity",
                        quantity);

                    cmd.Parameters.AddWithValue(
                        "@ProductPice",
                        productPice);

                    try
                    {
                        connection.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null &&
                            int.TryParse(
                                result.ToString(),
                                out int insertedID))
                        {
                            itemID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while adding order item",
                            "OrderItemDataAccess",
                            ex);

                        return -1;
                    }
                }
            }

            return itemID;
        }


        /// <summary>
        /// ID'si verilen OrderItem kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true, değilse false.
        /// </returns>
        public static bool UpdateOrderItem(
            int itemID,
            int sessionID,
            int productID,
            short quantity,
            short productPice)
        {
            int rowsAffected = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"
                    UPDATE OrderItems
                    SET
                        SessionID = @SessionID,
                        ProductID = @ProductID,
                        Quantity = @Quantity,
                        ProductPice = @ProductPice
                    WHERE ItemID = @ItemID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@ItemID",
                        itemID);

                    cmd.Parameters.AddWithValue(
                        "@SessionID",
                        sessionID);

                    cmd.Parameters.AddWithValue(
                        "@ProductID",
                        productID);

                    cmd.Parameters.AddWithValue(
                        "@Quantity",
                        quantity);

                    cmd.Parameters.AddWithValue(
                        "@ProductPice",
                        productPice);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while updating order item",
                            "OrderItemDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }


        /// <summary>
        /// ID'si verilen OrderItem kaydını siler.
        /// </summary>
        /// <param name="itemID">Silinecek Item ID</param>
        /// <returns>
        /// Delete işlemi başarılıysa true, değilse false.
        /// </returns>
        public static bool DeleteOrderItem(int itemID)
        {
            int rowsAffected = -1;

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query =
                    "DELETE OrderItems WHERE ItemID = @ItemID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@ItemID",
                        itemID);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            System.Diagnostics.EventLogEntryType.Warning,
                            "An Error occurred while deleting order item",
                            "OrderItemDataAccess",
                            ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
    }
}