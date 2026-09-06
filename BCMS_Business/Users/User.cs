using System;
using System.Data;

using BCMS_Data.Users;

namespace BCMS_Business.Users
{
    public class User
    {
        /// <summary>
        /// ID Database tarafından verildiği için dışardan set edilememeli.
        /// </summary>
        public int ID { get; private set; }

        //TODO: custom attribute'ta if sorunun çözdükten sonra sayısal değerler için de kontrol ekle.
        //Şu anki RequiredVariable sadece string değerler için çalışıyor."
        public int PersonID { get; set; }

        [Common.Attributes.RequiredVariable]
        public string UserName { get; set; }

       

        [Common.Attributes.RequiredVariable]
        public string Password { get; set; }

        /// <summary>
        /// Şifreyi 2 defa hash yapmamak adına. Update yaperken şifrenin gerçekten değişip değişmediğini kontrol etmeliyiz
        /// </summary>
        private static string previousPassword;
        
        [Common.Attributes.RequiredVariable]
        public bool IsActive { get; set; }


        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { get; private set; } = enMode.AddNew;


        /// <summary>
        /// User nesnesini oluşturur.
        /// </summary>
        /// <remarks>
        /// Bu constructor bilinçli olarak private bırakılmıştır. Amaç, sınıfın
        /// yalnızca kendi içinde (örn. bir factory metodu veya veritabanından
        /// okunan verilerle) örneklenebilmesini sağlamaktır.
        /// </remarks>
        private User(int ID, int personID, string userName, string password, bool isActive)
        {
            this.ID = ID;
            PersonID = personID;
            UserName = userName;
            Password = password;
            IsActive = isActive;
            previousPassword = password;
            Mode = enMode.Update;
        }


        /// <summary>
        /// Bu const ile dışardan da veri oluşturabilmek için public yaptık. Ama boş veri oluşturur.
        /// </summary>
        public User()
        {
            ID = -1;
            PersonID = -1;
            UserName = "";
            Password = "";
            IsActive = false;
            Mode = enMode.AddNew;
        }


        /// <summary>
        /// DB'deki tüm user verisini DL'dan alır ve datatable olarak return eder.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUserList()
        {
            return UserDataAccess.GetUsers();
        }


        /// <summary>
        /// ID ile arama yapar. Eğer DB'de veri varsa o veriyi objeye doldurur
        /// </summary>
        /// <param name="ID">Aranacak user ID'si</param>
        /// <returns>Eğer kayıt bulunabilirse user objesi eğer bulunamazsa null</returns>
        public static User Find(int ID)
        {
            string UserName = string.Empty, Password = string.Empty;
            int PersonID = -1;
            bool IsActive = false;
           

            if (UserDataAccess.Find(ID, ref PersonID, ref UserName, ref Password, ref IsActive))
            {
                return new User(ID, PersonID, UserName, Password, IsActive);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// User ID'sinin var olup olmadığını kontrol eder. Obje döndürmez.
        /// User varsa true yoksa false döner.
        /// </summary>
        /// <param name="ID">User ID to Check</param>
        /// <returns>Return true or false</returns>
        public static bool IsUserExists(int ID)
        {
            return UserDataAccess.IsUserExists(ID);
        }

        
        /// <summary>
        /// Şifreyi bu katmanda encrypt edilemli. Çünkü bu işlem PL veya DL ile alakası olmayan işlem. 
        /// Bu işlem bir logic'tir.
        /// </summary>
        /// <returns></returns>
        private string EncryptPassword()
        {
            return Common.Hashing.ComputeHash(this.Password);
        }

        /// <summary>
        /// Mode'u add olan user objesini DB'ye ekler.
        /// </summary>
        /// <returns>Geriye otomatik olarak SSMS tarafından verilen ID'yi döndürür</returns>
        private bool _AddNewUser()
        {
            if (!IsValid())
            {
                return false;
            }
            
            previousPassword = this.Password;
            this.ID = UserDataAccess.AddNewUser(this.PersonID, this.UserName, EncryptPassword(), this.IsActive);

            return (this.ID != -1);
        }


        /// <summary>
        /// Save metodu ile çağırılır. Eğer user DB'de varsa tüm alanlar yeni bilgiler ile güncellenir.
        /// </summary>
        /// <returns>Eğer update işlemi sorunsuz olduysa true</returns>
        private bool _UpdateUser()
        {
            if (!IsValid())
            {
                return false;
            }
            //if(UserDataAccess.GetUserPassword(this.ID)!=this.Password)
            //{
            //    this.Password = EncryptPassword();
            //}
         
            if(this.Password!=previousPassword)
            {
                this.Password = EncryptPassword();
            }
            return UserDataAccess.UpdateUser(this.ID, this.PersonID, this.UserName, this.Password, this.IsActive);
        }


        /// <summary>
        /// User DB'de varsa siler.
        /// </summary>
        /// <returns>Eğer silme işlemi başarılı olursa true</returns>
        public bool DeleteUser()
        {
            return UserDataAccess.DeleteUser(this.ID);
        }


        /// <summary>
        /// Yazdığımız Attribute kendi kendini kontrol edemez. Bu yüzden bir custom attribute yazdığımızda
        /// onu okuyabilecek olan kodu da yazmalıyız.
        /// </summary>
        /// <returns>Eğer not nullable tüm alanlar null değilse true döner</returns>
        private bool IsValid()
        {
            Type type = typeof(User);

            foreach (var prop in type.GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(Common.Attributes.RequiredVariableAttribute)))
                {
                    string value = prop.GetValue(this)?.ToString();

                    if (string.IsNullOrEmpty(value))
                    {
                        Console.WriteLine($"Validation Failed for property '{prop.Name}'");
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
                if (_AddNewUser())
                {
                    this.Mode = enMode.Update;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return _UpdateUser();
        }
    }
}