using System;
using System.Data;

using BCMS_Data.Tables;

namespace BCMS_Business.Tables
{
    public class Table
    {
        /// <summary>
        /// ID Database tarafından verildiği için dışardan set edilememeli.
        /// </summary>
        public int TableID { get; private set; }

        public int PriceID { get; set; }

        public enum enTableType { Caroom = 1, Snooker = 2 };
        public enTableType TableType { get;  set; } = enTableType.Caroom;


        public enum enTableStatus { Active = 1, Passive = 2, DefectiveTable = 3 };
        public enTableStatus TableStatus { get;  set; } = enTableStatus.Passive;

        public enum enMode { AddNew = 0, Update = 1 };

        public enMode Mode { get; private set; } = enMode.AddNew;


        /// <summary>
        /// DB'den bulunan Table nesnesini oluşturur.
        /// </summary>
        private Table(
            int tableID,
            int priceID,
            enTableType tableType,
            enTableStatus tableStatus)
        {
            TableID = tableID;
            PriceID = priceID;
            TableType = tableType;
            TableStatus = tableStatus;

            Mode = enMode.Update;
        }


        /// <summary>
        /// Yeni bir Table nesnesi oluşturur.
        /// </summary>
        public Table()
        {
            TableID = -1;
            PriceID = -1;
            TableType = 0;
            TableStatus = 0;

            Mode = enMode.AddNew;
        }


        /// <summary>
        /// DB'deki tüm Table verisini Data Access katmanından alır.
        /// </summary>
        /// <returns>Table kayıtlarını içeren DataTable</returns>
        public static DataTable GetTableList()
        {
            return TableDataAccess.GetTables();
        }


        /// <summary>
        /// ID ile arama yapar.
        /// Eğer DB'de veri varsa o veriyi objeye doldurur.
        /// </summary>
        /// <param name="tableID">Aranacak Table ID</param>
        /// <returns>
        /// Kayıt bulunursa Table objesi,
        /// bulunamazsa null döner.
        /// </returns>
        public static Table Find(int tableID)
        {
            int priceID = -1;
            byte tableType = 0;
            byte tableStatus=0;

            if (TableDataAccess.Find(
                tableID,
                ref priceID,
                ref tableType,
                ref tableStatus))
            {
                return new Table(
                    tableID,
                    priceID,
                    (enTableType)tableType,
                    (enTableStatus)tableStatus);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Table ID'nin DB'de olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="tableID">Kontrol edilecek Table ID</param>
        /// <returns>
        /// Kayıt varsa true, yoksa false döner.
        /// </returns>
        public static bool IsTableExists(int tableID)
        {
            return TableDataAccess.IsTableExists(tableID);
        }


        /// <summary>
        /// Mode'u AddNew olan Table objesini DB'ye ekler.
        /// </summary>
        /// <returns>
        /// İşlem başarılıysa true, değilse false döner.
        /// </returns>
        private bool _AddNewTable()
        {
            //if (!IsValid())
            //{
            //    return false;
            //}

            this.TableID = TableDataAccess.AddNewTable(
                this.PriceID,
               (byte)this.TableType,
                 (byte)this.TableStatus);

            return (this.TableID != -1);
        }


        /// <summary>
        /// DB'de bulunan Table kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true döner.
        /// </returns>
        private bool _UpdateTable()
        {

            return TableDataAccess.UpdateTable(
                this.TableID,
                this.PriceID,
                 (byte)this.TableType,
                 (byte)this.TableStatus);
        }


        /// <summary>
        /// Table kaydını DB'den siler.
        /// </summary>
        /// <returns>
        /// Silme işlemi başarılıysa true döner.
        /// </returns>
        public bool DeleteTable()
        {
            return TableDataAccess.DeleteTable(this.TableID);
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
                if (_AddNewTable())
                {
                    this.Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return _UpdateTable();
        }
    }
}