using BCMS_Data.Sessions;
using System;
using System.Data;
using static Common.Attributes;

namespace BCMS_Business.Sessions
{
    public class Session
    {
        /// <summary>
        /// ID Database tarafından verildiği için dışardan set edilememeli.
        /// </summary>
        public int SessionID { get; private set; }

        [Common.Attributes.PositiveInteger]
        public int CreatedByStaffID { get; set; }
        [Common.Attributes.PositiveInteger]
        public int TableID { get; set; }

        //TODO: Session date ve time sistem tarafında verilmesi gerekebilir. Çünkü ikigün sonra session açamazsın veya 2 gün önce session açamazsın. Bir session açtığında o anki zaman bilgisi ile açılır
        //Her ne kadar session sistem tarafından verilecek olsa da biz yinde date'in istediğimiz aralıkta olup olmaığını kontrol etmek istiyoruz
        [Common.Attributes.ValidateDate(0,0)]
        public DateTime SessionDate { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }

        public enum enSessionStatus { Active = 1, Cancelled = 2, };
        public enSessionStatus SessionStatus { set; get; } = enSessionStatus.Active;

        public enum enMode { AddNew = 0, Update = 1 };

        public enMode Mode { get; private set; } = enMode.AddNew;


        /// <summary>
        /// DB'den bulunan Session nesnesini oluşturur.
        /// </summary>
        private Session( int sessionID, int createdByStaffID,  int tableID,   DateTime sessionDate,DateTime sessionStartTime,  DateTime sessionEndTime,  enSessionStatus sessionStatus)
        {
            SessionID = sessionID;
            CreatedByStaffID = createdByStaffID;
            TableID = tableID;
            SessionDate = sessionDate;
            SessionStartTime = sessionStartTime;
            SessionEndTime = sessionEndTime;
            SessionStatus = sessionStatus;

            Mode = enMode.Update;
        }


        /// <summary>
        /// Yeni bir Session nesnesi oluşturur.
        /// </summary>
        public Session()
        {
            SessionID = -1;
            CreatedByStaffID = -1;
            TableID = -1;
            SessionDate = DateTime.MinValue;
            SessionStartTime = DateTime.MinValue;
            SessionEndTime = DateTime.MinValue;
            SessionStatus = enSessionStatus.Active;

            Mode = enMode.AddNew;
        }


        /// <summary>
        /// DB'deki tüm Session verisini Data Access katmanından alır.
        /// </summary>
        /// <returns>Session kayıtlarını içeren DataTable</returns>
        public static DataTable GetSessionList()
        {
            return SessionDataAccess.GetSessions();
        }


        /// <summary>
        /// ID ile arama yapar.
        /// Eğer DB'de veri varsa o veriyi objeye doldurur.
        /// </summary>
        /// <param name="sessionID">Aranacak Session ID</param>
        /// <returns>
        /// Kayıt bulunursa Session objesi,
        /// bulunamazsa null döner.
        /// </returns>
        public static Session Find(int sessionID)
        {
            int createdByStaffID = -1;
            int tableID = -1;

            DateTime sessionDate = DateTime.MinValue;
            DateTime sessionStartTime = DateTime.MinValue;
            DateTime sessionEndTime = DateTime.MinValue;

            byte sessionStatus = 0;


            if (SessionDataAccess.Find(
                sessionID,
                ref createdByStaffID,
                ref tableID,
                ref sessionDate,
                ref sessionStartTime,
                ref sessionEndTime,
                ref sessionStatus))
            {
                return new Session(
                    sessionID,
                    createdByStaffID,
                    tableID,
                    sessionDate,
                    sessionStartTime,
                    sessionEndTime,
                    (enSessionStatus)sessionStatus);
            }
            else
            {
                return null;
            }
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
            return SessionDataAccess.IsSessionExists(sessionID);
        }


        /// <summary>
        /// Mode'u AddNew olan Session objesini DB'ye ekler.
        /// </summary>
        /// <returns>
        /// İşlem başarılıysa true, değilse false döner.
        /// </returns>
        private bool _AddNewSession()
        {
            if (!IsValid())
            {
                return false;
            }

            this.SessionID = SessionDataAccess.AddNewSession(
                this.CreatedByStaffID,
                this.TableID,
                this.SessionDate,
                this.SessionStartTime,
                this.SessionEndTime,
                (byte)this.SessionStatus);

            return (this.SessionID != -1);
        }


        /// <summary>
        /// DB'de bulunan Session kaydını günceller.
        /// </summary>
        /// <returns>
        /// Update işlemi başarılıysa true döner.
        /// </returns>
        private bool _UpdateSession()
        {
            if (!IsValid())
            {
                return false;
            }

            return SessionDataAccess.UpdateSession(
                this.SessionID,
                this.CreatedByStaffID,
                this.TableID,
                this.SessionDate,
                this.SessionStartTime,
                this.SessionEndTime,
                (byte)this.SessionStatus);
        }


        /// <summary>
        /// Session kaydını DB'den siler.
        /// </summary>
        /// <returns>
        /// Silme işlemi başarılıysa true döner.
        /// </returns>
        public bool DeleteSession()
        {
            return SessionDataAccess.DeleteSession(this.SessionID);
        }


        /// <summary>
        /// Session nesnesinin geçerli olup olmadığını kontrol eder.
        /// </summary>
        /// <returns>
        /// Değerler geçerliyse true, değilse false döner.
        /// </returns>
        private bool IsValid()
        {
            Type type = typeof(Session);

            //tüm propları al
            foreach (var prop in type.GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(PropertiesValidationAttribute)))
                {
                    //Bu adımda ise "bir" property için tanımlanmış tüm attributları bir dizi halinde alıyoruz
                    object[] allAttributes = Attribute.GetCustomAttributes(prop, typeof(PropertiesValidationAttribute));

                    foreach (PropertiesValidationAttribute attribute in allAttributes)
                    {
                        //PropertiesValidationAttribute sayesinde her bir attrute kendi isValid fonksiyounu çağırıyoruz.
                        //bu sayede tanımladığımız attributeları ayrı ayrı kontrol etmek yerine
                        //PropertiesValidationAttribute'ı kontrol ediyoruz. Oda bir attribtute için //attribute'ın isValid fonksiyonun çağırıyor.
                        if (!attribute.IsValid(prop.GetValue(this), $"Validation Failed for Property {prop.Name}"))
                        {
                            return false;
                        }
                    }

                    if (true)
                    {

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
                if (_AddNewSession())
                {
                    this.Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return _UpdateSession();
        }
    }
}