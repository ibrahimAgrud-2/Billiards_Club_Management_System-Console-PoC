using System;
using System.Data;

using BCMS_Data.OrderItems;

namespace BCMS_Business.OrderItems
{
    public class OrderItem
    {
        /// <summary>
        /// ID Database tarafından verildiği için dışardan set edilememeli.
        /// </summary>
        public int ItemID { get; private set; }

        public int SessionID { get; set; }

        public int ProductID { get; set; }

        public short Quantity { get; set; }

        public short ProductPice { get; set; }


        public enum enMode { AddNew = 0, Update = 1 };

        public enMode Mode { get; private set; } = enMode.AddNew;


        /// <summary>
        /// DB'den bulunan OrderItem nesnesini oluşturur.
        /// </summary>
        private OrderItem(
            int itemID,
            int sessionID,
            int productID,
            short quantity,
            short productPice)
        {
            ItemID = itemID;
            SessionID = sessionID;
            ProductID = productID;
            Quantity = quantity;
            ProductPice = productPice;

            Mode = enMode.Update;
        }


        /// <summary>
        /// Yeni bir OrderItem nesnesi oluşturur.
        /// </summary>
        public OrderItem()
        {
            ItemID = -1;
            SessionID = -1;
            ProductID = -1;
            Quantity = 0;
            ProductPice = 0;

            Mode = enMode.AddNew;
        }


        /// <summary>
        /// DB'deki tüm OrderItem verisini Data Access katmanından alır.
        /// </summary>
        /// <returns>
        /// OrderItem kayıtlarını içeren DataTable
        /// </returns>
        public static DataTable GetOrderItemList()
        {
            return OrderItemDataAccess.GetOrderItems();
        }


        /// <summary>
        /// ID ile arama yapar.
        /// Eğer DB'de veri varsa o veriyi objeye doldurur.
        /// </summary>
        /// <param name="itemID">Aranacak Item ID</param>
        /// <returns>
        /// Kayıt bulunursa OrderItem objesi,
        /// bulunamazsa null döner.
        /// </returns>
        public static OrderItem Find(int itemID)
        {
            int sessionID = -1;
            int productID = -1;

            short quantity = 0;
            short productPice = 0;


            if (OrderItemDataAccess.Find(
                itemID,
                ref sessionID,
                ref productID,
                ref quantity,
                ref productPice))
            {
                return new OrderItem(
                    itemID,
                    sessionID,
                    productID,
                    quantity,
                    productPice);
            }
            else
            {
                return null;
            }
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
            return OrderItemDataAccess.IsOrderItemExists(itemID);
        }


        /// <summary>
        /// Mode'u AddNew olan OrderItem objesini DB'ye ekler.
        /// </summary>
        /// <returns>
        /// İşlem başarılıysa true, değilse false.
        /// </returns>
        private bool _AddNewOrderItem()
        {
            if (!IsValid())
            {
                return false;
            }

            this.ItemID = OrderItemDataAccess.AddNewOrderItem(
                this.SessionID,
                this.ProductID,
                this.Quantity,
                this.ProductPice);

            return (this.ItemID != -1);
        }


        /// <summary>
        /// DB'de bulunan OrderItem kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true döner.
        /// </returns>
        private bool _UpdateOrderItem()
        {
            if (!IsValid())
            {
                return false;
            }

            return OrderItemDataAccess.UpdateOrderItem(
                this.ItemID,
                this.SessionID,
                this.ProductID,
                this.Quantity,
                this.ProductPice);
        }


        /// <summary>
        /// OrderItem kaydını DB'den siler.
        /// </summary>
        /// <returns>
        /// Silme işlemi başarılıysa true, değilse false.
        /// </returns>
        public bool DeleteOrderItem()
        {
            return OrderItemDataAccess.DeleteOrderItem(this.ItemID);
        }


        /// <summary>
        /// OrderItem nesnesinin geçerli olup olmadığını kontrol eder.
        /// </summary>
        /// <returns>
        /// Değerler geçerliyse true, değilse false.
        /// </returns>
        private bool IsValid()
        {
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
                if (_AddNewOrderItem())
                {
                    this.Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return _UpdateOrderItem();
        }
    }
}