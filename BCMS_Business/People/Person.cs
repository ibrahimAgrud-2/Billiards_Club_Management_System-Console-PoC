using System;
using System.Data;

using  BCMS_Data.People;

namespace BCMS_Business.People
{
    public class Person
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }

  
        /// <summary>
        /// The variable is nullable. Therefore, you must check if it's null before using it
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// The variable is nullable. Therefore, you must check if its null or not before using it
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// The variable is nullable. Therefore, you must check if its null or not before using it
        /// </summary>
        public string Email { get; set; }

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;


        /// <summary>
        /// Person nesnesini oluşturur.
        /// </summary>
        /// <remarks>
        /// Bu constructor bilinçli olarak private bırakılmıştır. Amaç, sınıfın
        /// yalnızca kendi içinde (örn. bir factory metodu veya veritabanından
        /// okunan verilerle) örneklenebilmesini sağlamaktır. Constructor public
        /// olsaydı, çağıran kod veritabanında karşılığı olmayan rastgele
        /// verilerle bir Person oluşturabilir ve sistem var olmayan bir kişi
        /// üzerinden işlem yapabilirdi
        /// </remarks>
        public Person(int personID, string firstName,string lastName,  DateTime birthDate,  string phone,     string address, string imagePath,string email)
        {
            PersonID = personID;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Phone = phone;
            Address = address;
            ImagePath = imagePath;
            Email = email;
            Mode = enMode.Update;
        }

        /// <summary>
        /// Bu const ile dışardan da veri oluşturabilmek için public yaptık. Ama boş veri oluşturur.
        /// </summary>
        public Person()
        {
            PersonID = -1;
            FirstName = "";
            LastName = "";
            BirthDate = DateTime.MinValue;
            Phone = "";
            Address = "";
            ImagePath = "";
            Email = "";
            Mode = enMode.AddNew;
        }
        


        /// <summary>
        /// DB'deki tüm person verisini DL'dan alır ve datatable olark return eder.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetPersonList()
        {
            return PersonDataAccess.GetPeople();
        }
        public static Person Find(int personID)
        {

            //Nullable değişkenler string.Empty yerine null verdik. Çünkü string.Empty="" yani bir değer
         string FirstName = string.Empty, LastName = string.Empty, Phone = string.Empty, Address = null, ImagePath = null, Email = null;
         DateTime BirthDate = DateTime.Now;

      
            if(PersonDataAccess.Find(personID,ref FirstName, ref LastName, ref BirthDate, ref Phone, ref Address, ref ImagePath, ref Email))
            {
                //Yeni bir person nesnesi için sadece return person(...) demen yetmez. new kullanmalısın
                return new Person(personID, FirstName, LastName, BirthDate, Phone, Address, ImagePath, Email);

            }
            else
            {
                return null;
            }
        }

        public static bool IsPersonExists(int personID)
        {
            return PersonDataAccess.IsPersonExists(personID);
        }

    }
}
