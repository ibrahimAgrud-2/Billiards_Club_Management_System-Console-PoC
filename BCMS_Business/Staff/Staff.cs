
using System;
using System.Data;

using BCMS_Data.Staff;

namespace BCMS_Business.Staff
{
    public class Staff
    {
        /// <summary>
        /// ID Database tarafından verildiği için dışardan set edilememeli.
        /// </summary>
        public int StaffID { get; private set; }


        public int PersonID { get; set; }


        public decimal Salary { get; set; }

        [Common.Attributes.DateVariable]
        public DateTime HireDate { get; set; }

        public int CreatedByUserID { get; set; }

        /// <summary>
        /// The variable is nullable. Therefore, you must check if it's null before using it
        /// </summary>
        public DateTime? SeparationDate { get; set; }


        public bool StillWorking { get; set; }


        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { get; private set; } = enMode.AddNew;


        /// <summary>
        /// Staff nesnesini oluşturur.
        /// </summary>
        /// <remarks>
        /// Bu constructor bilinçli olarak private bırakılmıştır. Amaç, sınıfın
        /// yalnızca kendi içinde (örn. bir factory metodu veya veritabanından
        /// okunan verilerle) örneklenebilmesini sağlamaktır.
        /// </remarks>
        private Staff(int staffID, int personID, decimal salary, DateTime hireDate, int createdByUserID, DateTime? separationDate, bool stillWorking)
        {
            StaffID = staffID;
            PersonID = personID;
            Salary = salary;
            HireDate = hireDate;
            CreatedByUserID = createdByUserID;
            SeparationDate = separationDate;
            StillWorking = stillWorking;
            Mode = enMode.Update;
        }


        /// <summary>
        /// Bu const ile dışardan da veri oluşturabilmek için public yaptık. Ama boş veri oluşturur.
        /// </summary>
        public Staff()
        {
            StaffID = -1;
            PersonID = -1;
            Salary = 0;
            HireDate = DateTime.MinValue;
            CreatedByUserID = -1;
            SeparationDate = null;
            StillWorking = true;
            Mode = enMode.AddNew;
        }


        /// <summary>
        /// DB'deki tüm staff verisini DL'dan alır ve datatable olarak return eder.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetStaffList()
        {
            return StaffDataAccess.GetStaff();
        }


        /// <summary>
        /// ID ile arama yapar. Eğer DB'de veri varsa o veriyi objeye doldurur.
        /// </summary>
        /// <param name="staffID">Aranacak staff ID'si</param>
        /// <returns>Eğer kayıt bulunabilirse staff objesi eğer bulunamazsa null</returns>
        public static Staff Find(int staffID)
        {
            int PersonID = -1;
            decimal Salary = 0;
            DateTime HireDate = DateTime.Now;
            int CreatedByUserID = -1;
            DateTime? SeparationDate = null;
            bool StillWorking = false;


            if (StaffDataAccess.Find(staffID, ref PersonID, ref Salary, ref HireDate, ref CreatedByUserID, ref SeparationDate, ref StillWorking))
            {
                return new Staff(staffID, PersonID, Salary, HireDate, CreatedByUserID, SeparationDate, StillWorking);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Staff ID'sinin var olup olmadığını kontrol eder. Obje döndürmez.
        /// Staff varsa true yoksa false döner.
        /// </summary>
        /// <param name="staffID">Staff ID to Check</param>
        /// <returns>Return true or false</returns>
        public static bool IsStaffExists(int staffID)
        {
            return StaffDataAccess.IsStaffExists(staffID);
        }


        /// <summary>
        /// Mode'u add olan staff objesini DB'ye ekler.
        /// </summary>
        /// <returns>Geriye otomatik olarak SSMS tarafından verilen ID'yi döndürür</returns>
        private bool _AddNewStaff()
        {
            if (!IsValid())
            {
                return false;
            }

            this.StaffID = StaffDataAccess.AddNewStaff(
                this.PersonID,
                this.Salary,
                this.HireDate,
                this.CreatedByUserID,
                this.SeparationDate,
                this.StillWorking);

            return (this.StaffID != -1);
        }


        /// <summary>
        /// Save metodu ile çağırılır. Eğer staff DB'de varsa tüm alanlar yeni bilgiler ile güncellenir.
        /// </summary>
        /// <returns>Eğer update işlemi sorunsuz olduysa true</returns>
        private bool _UpdateStaff()
        {
            if (!IsValid())
            {
                return false;
            }

            return StaffDataAccess.UpdateStaff(
                this.StaffID,
                this.PersonID,
                this.Salary,
                this.HireDate,
                this.CreatedByUserID,
                this.SeparationDate,
                this.StillWorking);
        }


        /// <summary>
        /// Staff DB'de varsa siler.
        /// </summary>
        /// <returns>Eğer silme işlemi başarılı olursa true</returns>
        public bool DeleteStaff()
        {
            return StaffDataAccess.DeleteStaff(this.StaffID);
        }


        /// <summary>
        /// Yazdığımız Attribute kendi kendini kontrol edemez. Bu yüzden bir custom attribute yazdığımızda
        /// onu okuyabilecek olan kodu da yazmalıyız. Mesela RequiredVariableAttribute attribute'ını okuyabilen
        /// bir fonksiyon yazarak o attribute'u anlamlı hale getirdik.
        /// </summary>
        /// <returns>Eğer required tüm alanlar geçerliyse true döner</returns>
        private bool IsValid()
        {
            Type type = typeof(Staff);

            foreach (var prop in type.GetProperties())
            {
               
                if (Attribute.IsDefined(prop, typeof(Common.Attributes.DateVariableAttribute)))
                {
                    var DateAttribute = (Common.Attributes.DateVariableAttribute)Attribute.GetCustomAttribute(
                        prop,
                        typeof(Common.Attributes.DateVariableAttribute));

                    DateTime date = Convert.ToDateTime(prop.GetValue(this));

                    if (date < DateAttribute.MinDate || date > DateAttribute.MaxDate)
                    {
                        Console.WriteLine(
                            $"Validation Failed for property '{prop.Name}'. Minimum date is {DateAttribute.MinDate.ToShortDateString()} and maximum date is {DateAttribute.MaxDate.ToShortDateString()}");

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
                if (_AddNewStaff())
                {
                    this.Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return _UpdateStaff();
        }
    }
}

    