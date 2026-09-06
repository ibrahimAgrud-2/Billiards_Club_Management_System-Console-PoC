using System;
using System.Data;

using BCMS_Data.Products;

namespace BCMS_Business.Products
{
    public class Product
    {
        /// <summary>
        /// ID Database tarafından verildiği için dışardan set edilememeli.
        /// </summary>
        public int ProductID { get; private set; }

        [Common.Attributes.RequiredVariable]
        public string ProductName { get; set; }

        /// <summary>
        /// The variable is nullable. Therefore, you must check if it's null before using it
        /// </summary>
        public string ImagePath { get; set; }

        [Common.Attributes.RequiredVariable]
        public decimal Price { get; set; }


        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { get; private set; } = enMode.AddNew;


        /// <summary>
        /// Product nesnesini oluşturur.
        /// </summary>
        /// <remarks>
        /// Bu constructor bilinçli olarak private bırakılmıştır. Amaç, sınıfın
        /// yalnızca kendi içinde (örn. bir factory metodu veya veritabanından
        /// okunan verilerle) örneklenebilmesini sağlamaktır.
        /// </remarks>
        private Product(int productID, string productName, string imagePath, decimal price)
        {
            ProductID = productID;
            ProductName = productName;
            ImagePath = imagePath;
            Price = price;
            Mode = enMode.Update;
        }


        /// <summary>
        /// Bu const ile dışardan da veri oluşturabilmek için public yaptık. Ama boş veri oluşturur.
        /// </summary>
        public Product()
        {
            ProductID = -1;
            ProductName = "";
            ImagePath = "";
            Price = 0;
            Mode = enMode.AddNew;
        }


        /// <summary>
        /// DB'deki tüm product verisini DL'dan alır ve datatable olarak return eder.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetProductList()
        {
            return ProductDataAccess.GetProducts();
        }


        /// <summary>
        /// ID ile arama yapar. Eğer DB'de veri varsa o veriyi objeye doldurur.
        /// </summary>
        /// <param name="productID">Aranacak ürün ID'si</param>
        /// <returns>Eğer kayıt bulunabilirse product objesi eğer bulunamazsa null</returns>
        public static Product Find(int productID)
        {
            string ProductName = string.Empty;
            string ImagePath = null;
            decimal Price = 0;


            if (ProductDataAccess.Find(productID, ref ProductName, ref ImagePath, ref Price))
            {
                return new Product(productID, ProductName, ImagePath, Price);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Product ID'sinin var olup olmadığını kontrol eder. Obje döndürmez.
        /// Product varsa true yoksa false döner.
        /// </summary>
        /// <param name="productID">Product ID to Check</param>
        /// <returns>Return true or false</returns>
        public static bool IsProductExists(int productID)
        {
            return ProductDataAccess.IsProductExists(productID);
        }


        /// <summary>
        /// Mode'u add olan product objesini DB'ye ekler.
        /// </summary>
        /// <returns>Geriye otomatik olarak SSMS tarafından verilen ID'yi döndürür</returns>
        private bool _AddNewProduct()
        {
            if (!IsValid())
            {
                return false;
            }

            this.ProductID = ProductDataAccess.AddNewProduct(
                this.ProductName,
                this.ImagePath,
                this.Price);

            return (this.ProductID != -1);
        }


        /// <summary>
        /// Save metodu ile çağırılır. Eğer product DB'de varsa tüm alanlar yeni bilgiler ile güncellenir.
        /// </summary>
        /// <returns>Eğer update işlemi sorunsuz olduysa true</returns>
        private bool _UpdateProduct()
        {
            if (!IsValid())
            {
                return false;
            }

            return ProductDataAccess.UpdateProduct(
                this.ProductID,
                this.ProductName,
                this.ImagePath,
                this.Price);
        }


        /// <summary>
        /// Product DB'de varsa siler.
        /// </summary>
        /// <returns>Eğer silme işlemi başarılı olursa true</returns>
        public bool DeleteProduct()
        {
            return ProductDataAccess.DeleteProduct(this.ProductID);
        }


        /// <summary>
        /// Yazdığımız Attribute kendi kendini kontrol edemez. Bu yüzden bir custom attribute yazdığımızda
        /// onu okuyabilecek olan kodu da yazmalıyız. Mesela RequiredVariableAttribute attribute'ını okuyabilen
        /// bir fonksiyon yazarak o attribute'u anlamlı hale getirdik.
        /// </summary>
        /// <returns>Eğer required tüm alanlar geçerliyse true döner</returns>
        private bool IsValid()
        {
            Type type = typeof(Product);

            foreach (var prop in type.GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(Common.Attributes.RequiredVariableAttribute)))
                {
                    string value = prop.GetValue(this)?.ToString();

                    if (string.IsNullOrEmpty(value))
                    {
                        Console.WriteLine($"Validation Failed for property '{prop.Name}'");
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Update ve Add işlemleri bu fonksiyondan çağrılır. Mode eğer add ise o obje için add
        /// fonksiyonunu çağırır. Değilse o obje için update fonksiyonunu çağırır.
        /// </summary>
        /// <returns>Eğer Add/Update işlemi hatasız olursa true döner</returns>
        public bool Save()
        {
            if (this.Mode == enMode.AddNew)
            {
                if (_AddNewProduct())
                {
                    this.Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return _UpdateProduct();
        }
    }
}

