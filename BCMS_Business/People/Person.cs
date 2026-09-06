    using System;
    using System.Data;

    using  BCMS_Data.People;

    namespace BCMS_Business.People
    {
        public class Person
        {

            /// <summary>
            /// ID Database tarafından verildiği için dışardan set edilememeli.
            /// </summary>
            public int PersonID { get; private set; }

            [Common.Attributes.RequiredVariable]
            public string FirstName { get; set; }

            [Common.Attributes.RequiredVariable]
            public string LastName { get; set; }
            public string FullName { get { return FirstName + " " + LastName; } }

            [Common.Attributes.DateVariable]
            public DateTime BirthDate { get; set; }

            [Common.Attributes.RequiredVariable]
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
            public enMode Mode { get; private set; } = enMode.AddNew;


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
            private Person(int personID, string firstName,string lastName,  DateTime birthDate,  string phone,     string address, string imagePath,string email)
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
        
        
            /// <summary>
            /// ID ile arama yapar. Eğer DB'de veri varsa o veriyi objeye doldurur
            /// </summary>
            /// <param name="personID">Aranacak kişi ID'si</param>
            /// <returns>Eğer kayıt bulunabilirse person objesi eğer bulunamazsa null</returns>
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

            /// <summary>
            /// Person ID person'un var olup olmadığını kontrl eder. Obje döndürmez. Kişi varsa true yoksa falsa döner.
            /// </summary>
            /// <param name="a">Person ID to Check</param>
            /// <returns>Return true or false</returns>
            public static bool IsPersonExists(int personID)
            {
                return PersonDataAccess.IsPersonExists(personID);
            }

            /// <summary>
            /// Mode'u add olan person objesini DB'ye ekler.
            /// </summary>
            /// <returns>Geriye Otomatik olarak SSMS tarafından verilen ID'i dönderir</returns>
            private bool _AddNewPerson()
            {
                if(!IsValid())
                {
                    return false;
                }

                this.PersonID = PersonDataAccess.AddNewPerson(this.FirstName, this.LastName, this.BirthDate, this.Phone, this.Address, this.Email, this.ImagePath);

                return (this.PersonID != -1);
            }

            /// <summary>
            /// Save metodu ile çağırılır. Eğer person DB'de varsa tüm alanlar yeni bilgiler ile güncellenir
            /// </summary>
            /// <returns>Eğer update işlemi sorunsuz olduysa true</returns>
            private bool _UpdatePerson()
            {
                if(!IsValid())
                {
                    return false;
                }
                return PersonDataAccess.UpdatePerson(this.PersonID,this.FirstName, this.LastName, this.BirthDate, this.Phone, this.Address, this.Email, this.ImagePath);
            }

            /// <summary>
            /// person DB'de varsa siler.
            /// </summary>
            /// <returns>Eğer silme işlemi başarılı olursa true</returns>
            public bool DeletePerson()
            {
                return PersonDataAccess.DeletePerson(this.PersonID);
            }

            /// <summary>
            /// Yazdığımız Attirbute kendi kendini kontrol edemez. Bu yüzden bir custom attribute yazdığımızda
            /// Onu okuyabilecek olan kodu da yazmakıyız. Mesela RequiredVariableAttribute attribute'ını okuyabilen 
            /// bir fonksiypn yazarak o attribute'u anlamlı hale getirdik
            /// </summary>
            /// <returns>Eğer not nullable tüm alanlar null değilse true döner</returns>
            private bool IsValid()
            {
                Type type = typeof(Person);

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
                    if (Attribute.IsDefined(prop, typeof(Common.Attributes.DateVariableAttribute)))
                    {
                        var DateAttribute = (Common.Attributes.DateVariableAttribute)Attribute.GetCustomAttribute(prop, typeof(Common.Attributes.DateVariableAttribute));

                        DateTime birth = Convert.ToDateTime(prop.GetValue(this));
                   
                            if (birth < DateAttribute.MinDate || birth > DateAttribute.MaxDate)
                            {
                                Console.WriteLine($"Validation Failed for property '{prop.Name}'. Minimum date is  {DateAttribute.MinDate.ToShortDateString()} and maximum date is {DateAttribute.MaxDate.ToShortDateString()}");
                                return false;
                            }
                    }
                }
                return true;
            }
        
            /// <summary>
            /// Update ve Add işlemleri bu fonksiyondan çağrılır. Mode eğer add ise o obje için add fonksiyonun çağırır. Değilse  o obje için update fonksiyonunu çağırır.
            /// </summary>
            /// <returns>Eğer Add/Update işlemi hatasız olursa true döner</returns>
            public bool Save()
            {
                if (this.Mode==enMode.AddNew)
                {
                    if (_AddNewPerson())
                    {
                        this.Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
        
                    return _UpdatePerson();
            

            }


        }
    }
