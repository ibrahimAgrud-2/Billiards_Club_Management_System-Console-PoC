using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BCMS_Data;
using BCMS_Data.People;

namespace BCMS_Business.People
{
    public class Person
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ImagePath { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Person nesnesini oluşturur.
        /// </summary>
        /// <remarks>
        /// Bu constructor bilinçli olarak private bırakılmıştır. Amaç, sınıfın
        /// yalnızca kendi içinde (örn. bir factory metodu veya veritabanından
        /// okunan verilerle) örneklenebilmesini sağlamaktır. Constructor public
        /// olsaydı, çağıran kod veritabanında karşılığı olmayan rastgele
        /// verilerle bir Person oluşturabilir ve sistem var olmayan bir kişi
        /// üzerinden işlem yapabilirdi.
        /// </remarks>
        private Person()
        {

        }



        public static DataTable GetPersonList()
        {
            return PersonDataAccess.GetPeople();
        }

    }
}
