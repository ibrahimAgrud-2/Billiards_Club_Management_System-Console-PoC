
using BCMS_Business.People;
using BCMS_Business.Users;
using BCMS_Data;
using System;
using System.Data;
using System.Net;
using System.Security.Policy;
using static Common.Attributes;

namespace BCMS_Business.Staff
{
    public class Staff:Person
    {
        /// <summary>
        /// ID Database tarafından verildiği için dışardan set edilememeli.
        /// </summary>
        public int StaffID { get; private set; }


        [Common.Attributes.SalaryAttribute]
        public decimal Salary { get; set; }

        [Common.Attributes.ValidateDate(-65,0)]
        public DateTime HireDate { get; set; }

        [Common.Attributes.PositiveInteger]
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
        private Staff(int staffID, int personID, string firstName, string lastName,
                     DateTime birthDate, string phone, string address,
                     string imagePath, string email, decimal salary, DateTime hireDate, int createdByUserID, DateTime? separationDate, bool stillWorking)
        {
            StaffID = staffID;
            PersonID = personID;
            Salary = salary;
            HireDate = hireDate;
            CreatedByUserID = createdByUserID;
            SeparationDate = separationDate;
            StillWorking = stillWorking;
            this.FirstName = firstName;
            this.Address = address;
            this.Email = email;
            this.LastName = lastName;
            this.Phone = phone;
            this.ImagePath = imagePath;
            this.BirthDate = birthDate;
            this.Mode = enMode.Update;
            this.PersonID = personID;
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
            int personID = -1;
            decimal salary = 0;
            DateTime hireDate = DateTime.Now;
            int createdByUserID = -1;
            DateTime? separationDate = null;
            bool stillWorking = false;


            if (StaffDataAccess.Find(staffID, ref personID, ref salary, ref hireDate, ref createdByUserID, ref separationDate, ref stillWorking))
            {
                Person p = Person.Find(personID);
                return new Staff(staffID, personID, p.FirstName, p.LastName, p.BirthDate, p.Phone, p.Address, p.ImagePath, p.Email,salary,hireDate,createdByUserID,separationDate,stillWorking);
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

        private bool IsValid()
        {
            Type type = typeof(Staff);

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
        /// Mode'u add olan staff objesini DB'ye ekler.
        /// </summary>
        /// <returns>Geriye otomatik olarak SSMS tarafından verilen ID'yi döndürür</returns>
        private bool _AddNewStaff()
        {
            if(!IsValid())
            {
                return false;
            }
            this.StaffID = StaffDataAccess.AddNewStaff(
                this.PersonID,
                this.Salary,
                DateTime.Now,
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

            if (this.HireDate<DateTime.Now.AddYears(-50)||this.HireDate>DateTime.Now)
            {
                return false;
            }


            //TODO: update yaparken'de add yaparken de userID o anki sisteme hangi user giriş yapmışsa onun ID'si verilmeli.
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
        /// Update ve Add işlemleri bu fonksiyondan çağrılır. Mode eğer add ise o obje için add
        /// fonksiyonunu çağırır. Değilse o obje için update fonksiyonunu çağırır.
        /// </summary>
        /// <returns>Eğer Add/Update işlemi hatasız olursa true döner</returns>
        public bool Save()
        {

            //Add/update işlemlerini save ile yaparız. Bu yüzden staff'ı save etmeden önce person'ı save ederiz. Sonra staff save edilir. Çünkü staff person'u miras olmıştır.
            base.Mode = (Person.enMode)Mode;
            if (!base.Save())
            {
                return false;
                //Common.Logger.Log(System.Diagnostics.EventLogEntryType.Error,"Person Could Not added. For User");
            }


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

    