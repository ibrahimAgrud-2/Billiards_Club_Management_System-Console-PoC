using BCMS_Business.People;
using BCMS_Data;

using System;
using System.Data;

namespace BCMS_Business
{
    public class TablePrices
    {
        /// <summary>
        /// ID Database tarafından verildiği için dışardan set edilememeli.
        /// </summary>
        public int PriceID { get; private set; }

        public int CreatedByUserID { get; set; }

        [Common.Attributes.RequiredVariable]
        public string Description { get; set; }

        public decimal PricePerHour { get; set; }


        public enum enMode { AddNew = 0, Update = 1 };

        public enMode Mode { get; private set; } = enMode.AddNew;


        /// <summary>
        /// DB'den bulunan TablePrices nesnesini oluşturur.
        /// </summary>
        private TablePrices( int priceID,   int createdByUserID,   string description,  decimal pricePerHour)
        {
            PriceID = priceID;
            CreatedByUserID = createdByUserID;
            Description = description;
            PricePerHour = pricePerHour;

            Mode = enMode.Update;
        }


        /// <summary>
        /// Yeni bir TablePrices nesnesi oluşturur.
        /// </summary>
        public TablePrices()
        {
            PriceID = -1;
            CreatedByUserID = -1;
            Description = "";
            PricePerHour = 0;

            Mode = enMode.AddNew;
        }


        /// <summary>
        /// DB'deki tüm TablePrices verisini Data Access katmanından alır.
        /// </summary>
        /// <returns>TablePrices kayıtlarını içeren DataTable</returns>
        public static DataTable GetTablePricesList()
        {
            return TablePricesDataAccess.GetPrices();
        }


        /// <summary>
        /// ID ile arama yapar.
        /// Eğer DB'de kayıt varsa o kaydı objeye doldurur.
        /// </summary>
        /// <param name="priceID">Aranacak TablePrices ID</param>
        /// <returns>
        /// Kayıt bulunursa TablePrices objesi,
        /// bulunamazsa null döner.
        /// </returns>
        public static TablePrices Find(int priceID)
        {
            int createdByUserID = -1;
            string description = null;
            decimal pricePerHour = 0;

            if (TablePricesDataAccess.Find(
                priceID,
                ref createdByUserID,
                ref description,
                ref pricePerHour))
            {
                return new TablePrices(
                    priceID,
                    createdByUserID,
                    description,
                    pricePerHour);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// TablePrices ID'nin DB'de olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="priceID">Kontrol edilecek TablePrices ID</param>
        /// <returns>
        /// Kayıt varsa true, yoksa false döner.
        /// </returns>
        public static bool IsTablePricesExists(int priceID)
        {
            return TablePricesDataAccess.IsPriceExists(priceID);
        }


        /// <summary>
        /// Mode'u AddNew olan TablePrices objesini DB'ye ekler.
        /// </summary>
        /// <returns>
        /// İşlem başarılıysa true, değilse false döner.
        /// </returns>
        private bool _AddNewTablePrices()
        {
            if (!IsValid())
            {
                return false;
            }

            this.PriceID = TablePricesDataAccess.AddNewPrice(
                this.CreatedByUserID,
                this.Description,
                this.PricePerHour);

            return (this.PriceID != -1);
        }


        /// <summary>
        /// DB'de bulunan TablePrices kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true döner.
        /// </returns>
        private bool _UpdateTablePrices()
        {
            if (!IsValid())
            {
                return false;
            }
            return TablePricesDataAccess.UpdatePrice(
                this.PriceID,
                this.CreatedByUserID,
                this.Description,
                this.PricePerHour);
        }


        /// <summary>
        /// TablePrices kaydını DB'den siler.
        /// </summary>
        /// <returns>
        /// Silme işlemi başarılıysa true döner.
        /// </returns>
        public bool DeleteTablePrices()
        {
            return TablePricesDataAccess.DeletePrice(this.PriceID);
        }


        /// <summary>
        /// TablePrices nesnesinin geçerli olup olmadığını kontrol eder.
        /// </summary>
        /// <returns>
        /// Değerler geçerliyse true, değilse false.
        /// </returns>
        private bool IsValid()
        {
            Type type = typeof(TablePrices);

            foreach (var prop in type.GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(Common.Attributes.RequiredVariableAttribute)))
                {

                    string value = prop.GetValue(this)?.ToString();

                    if (string.IsNullOrEmpty(value))
                    {
                        //prop.name=o an kontrol ettiğimiz prop adı. Kişi adı değil
                        Console.WriteLine($"Validation Failed for property '{prop.Name}'");
                        return false;
                    }
                } 
            }
            return true;
        }


        /// <summary>
        /// Add veya Update işlemini gerçekleştirir.
        /// Mode AddNew ise yeni kayıt ekler,
        /// Update ise mevcut kaydı günceller.
        /// </summary>
        /// <returns>
        /// İşlem başarılıysa true, değilse false döner.
        /// </returns>
        public bool Save()
        {
            if (this.Mode == enMode.AddNew)
            {
                if (_AddNewTablePrices())
                {
                    this.Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return _UpdateTablePrices();
        }
    }
}