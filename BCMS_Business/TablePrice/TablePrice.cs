using BCMS_Business.People;
using BCMS_Data;

using System;
using System.Data;

namespace BCMS_Business
{
    public class TablePrice
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
        /// DB'den bulunan TablePrice nesnesini oluşturur.
        /// </summary>
        private TablePrice( int priceID,   int createdByUserID,   string description,  decimal pricePerHour)
        {
            PriceID = priceID;
            CreatedByUserID = createdByUserID;
            Description = description;
            PricePerHour = pricePerHour;

            Mode = enMode.Update;
        }


        /// <summary>
        /// Yeni bir TablePrice nesnesi oluşturur.
        /// </summary>
        public TablePrice()
        {
            PriceID = -1;
            CreatedByUserID = -1;
            Description = "";
            PricePerHour = 0;

            Mode = enMode.AddNew;
        }


        /// <summary>
        /// DB'deki tüm TablePrice verisini Data Access katmanından alır.
        /// </summary>
        /// <returns>TablePrice kayıtlarını içeren DataTable</returns>
        public static DataTable GetTablePriceList()
        {
            return TablePriceDataAccess.GetPrices();
        }


        /// <summary>
        /// ID ile arama yapar.
        /// Eğer DB'de kayıt varsa o kaydı objeye doldurur.
        /// </summary>
        /// <param name="priceID">Aranacak TablePrice ID</param>
        /// <returns>
        /// Kayıt bulunursa TablePrice objesi,
        /// bulunamazsa null döner.
        /// </returns>
        public static TablePrice Find(int priceID)
        {
            int createdByUserID = -1;
            string description = null;
            decimal pricePerHour = 0;

            if (TablePriceDataAccess.Find(
                priceID,
                ref createdByUserID,
                ref description,
                ref pricePerHour))
            {
                return new TablePrice(
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
        /// TablePrice ID'nin DB'de olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="priceID">Kontrol edilecek TablePrice ID</param>
        /// <returns>
        /// Kayıt varsa true, yoksa false döner.
        /// </returns>
        public static bool IsTablePriceExists(int priceID)
        {
            return TablePriceDataAccess.IsPriceExists(priceID);
        }


        /// <summary>
        /// Mode'u AddNew olan TablePrice objesini DB'ye ekler.
        /// </summary>
        /// <returns>
        /// İşlem başarılıysa true, değilse false döner.
        /// </returns>
        private bool _AddNewTablePrice()
        {
            if (!IsValid())
            {
                return false;
            }

            this.PriceID = TablePriceDataAccess.AddNewPrice(
                this.CreatedByUserID,
                this.Description,
                this.PricePerHour);

            return (this.PriceID != -1);
        }


        /// <summary>
        /// DB'de bulunan TablePrice kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true döner.
        /// </returns>
        private bool _UpdateTablePrice()
        {
            if (!IsValid())
            {
                return false;
            }
            return TablePriceDataAccess.UpdatePrice(
                this.PriceID,
                this.CreatedByUserID,
                this.Description,
                this.PricePerHour);
        }


        /// <summary>
        /// TablePrice kaydını DB'den siler.
        /// </summary>
        /// <returns>
        /// Silme işlemi başarılıysa true döner.
        /// </returns>
        public bool DeleteTablePrice()
        {
            return TablePriceDataAccess.DeletePrice(this.PriceID);
        }


        /// <summary>
        /// TablePrice nesnesinin geçerli olup olmadığını kontrol eder.
        /// </summary>
        /// <returns>
        /// Değerler geçerliyse true, değilse false.
        /// </returns>
        private bool IsValid()
        {
            Type type = typeof(TablePrice);

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
                if (_AddNewTablePrice())
                {
                    this.Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return _UpdateTablePrice();
        }
    }
}