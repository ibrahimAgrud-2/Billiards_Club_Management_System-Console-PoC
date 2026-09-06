using Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCMS_Data.Products
{
    public class ProductDataAccess
    {

        /// <summary>
        /// DB'deki Product tablosundaki tüm tabloyu olduğu gibi alır
        /// </summary>
        /// <returns>Product data table</returns>
        public static DataTable GetProducts()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "select * from Products ";

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
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while getting products", "ProductDataAccess", ex);
                    }
                }
            }

            return dt;
        }


        /// <summary>
        /// Eğer ürünü bulursa her bir parametreye ürünün verisini yükler.
        /// </summary>
        /// <returns>Ürün varsa true, yoksa false</returns>
        public static bool Find(int productID, ref string productName, ref string imagePath, ref decimal price)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "select * from Products where ProductID=@ProductID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                productName = read["ProductName"].ToString();
                                imagePath = read["ImagePath"]?.ToString() ?? null;
                                price = Convert.ToDecimal(read["TablePrices"]);

                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while finding product", "ProductDataAccess", ex);
                        return false;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Ürünün olup olmadığını kontrol eder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>Eğer ürün DB'de varsa true, yoksa false döner</returns>
        public static bool IsProductExists(int productID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = "SELECT Found=1 FROM Products WHERE ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);

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
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while checking for product existence", "ProductDataAccess", ex);

                        return false;
                    }
                }
            }

            return isFound;
        }


        /// <summary>
        /// Add Product to database
        /// </summary>
        /// <returns>Otomatik olarak verilen product IDsi</returns>
        public static int AddNewProduct(string productName, string imagePath, decimal price)
        {
            int productID = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"INSERT INTO Products (ProductName, ImagePath, Price)
                                 VALUES (@ProductName, @ImagePath, @Price);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductName", productName);

                    if (!string.IsNullOrEmpty(imagePath))
                        cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    else
                        cmd.Parameters.AddWithValue("@ImagePath", System.DBNull.Value);

                    cmd.Parameters.AddWithValue("@Price", price);

                    try
                    {
                        connection.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            productID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while adding product", "ProductDataAccess", ex);

                        return -1;
                    }
                }
            }

            return productID;
        }


        /// <summary>
        /// ID ile product tüm alanları günceller.
        /// </summary>
        /// <returns>Eğer update yaparken sorun çıkmazsa true</returns>
        public static bool UpdateProduct(int productID, string productName, string imagePath, decimal price)
        {
            int rowsAffected = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"Update Products
                                 set ProductName = @ProductName,
                                     ImagePath = @ImagePath,
                                     Price = @Price
                                 where ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    cmd.Parameters.AddWithValue("@ProductName", productName);

                    if (!string.IsNullOrEmpty(imagePath))
                        cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    else
                        cmd.Parameters.AddWithValue("@ImagePath", System.DBNull.Value);

                    cmd.Parameters.AddWithValue("@Price", price);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while updating product", "ProductDataAccess", ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }


        /// <summary>
        /// ID'sini verdiğin ürünü siler.
        /// </summary>
        /// <param name="productID">Silinecek ürünün ID'si</param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID)
        {
            int rowsAffected = -1;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                string query = @"delete Products where ProductID=@ProductID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);

                    try
                    {
                        connection.Open();

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(System.Diagnostics.EventLogEntryType.Warning, "An Error occurred while deleting product", "ProductDataAccess", ex);

                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
    }
}