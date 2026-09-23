using System;


namespace Common
{

    /// <summary>
    /// Bu sınıf içinde, sistemde olmasını istediğimiz tüm attributlar alt sınıf olarak bulunur
    /// </summary>
    public class Attributes
    {

        /// <summary>
        /// Bu attribute null olmaması gereken proplar içindir
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class RequiredVariableAttribute:Attribute
        {  
          
        }

      

        /// <summary>
        /// Bu attribute date'in min 18 max 65 yılında olması gerekiğinde kullanılır.
        /// Date mutlaka 'gün-ay-yıl' formatında olmalıdır.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class BirthDateVariableAttribute:Attribute
        {

            public DateTime MinDate;
            public DateTime MaxDate;
            public BirthDateVariableAttribute()
            {

                this.MaxDate = DateTime.Now.AddYears(-18);
                this.MinDate = DateTime.Now.AddYears(-65);
            }
        }

     //*********************************************************
        /*
    * biz PropertiesValidationAttribute'i yani ana attribute'ı şu yüzden tanımladık: her bir attribute türünü ayrı ayrı kontrol etmek yerine bir ana attribute'u kontrol ederiz. O ana attribute alt attributeların isValid fonksyionlarını çağıracak.
    */
        [AttributeUsage(AttributeTargets.Property)]
        public abstract class PropertiesValidationAttribute : Attribute
        {
            public abstract bool IsValid(object value, string message);
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class RequiredFieldAttribute : PropertiesValidationAttribute
        {
            public override bool IsValid(object value, string message)
            {
                if (value == null)
                {
                    Console.WriteLine(message);
                    return false;
                }
                return true;
            }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class BirthDateValidationAttribute : PropertiesValidationAttribute
        {

            public DateTime MinDate;
            public DateTime MaxDate;
            /// <summary>
            /// Eğer min date verilmemişse default olarak -70 yapıyoruz. yani en erken 1964 yılında doğmuş olabilir
            /// Aynı şey max içinde geçerli. Eğer değer verilmemişse. Doğum günü -18 yani en geç 2008'e ayarlanıyor.
            /// Yani değerler boşsa default doğun tarihi (sistemin kabul ettiği) 1960-2008 arasında oluyor.
            /// <param name="minDate"></param>
            /// <param name="maxDate"></param>
            public BirthDateValidationAttribute(string minDate = "", string maxDate = "")
            {
                if (string.IsNullOrEmpty(minDate))
                {
                    minDate = DateTime.Now.AddYears(-70).ToString();
                }
                if (string.IsNullOrEmpty(maxDate))
                {
                    maxDate = DateTime.Now.AddYears(-18).ToString();
                }

                this.MaxDate = Convert.ToDateTime(maxDate);
                this.MinDate = Convert.ToDateTime(minDate);
            }
            public override bool IsValid(object value, string message)
            {

                DateTime birthDate = Convert.ToDateTime(value);
                if (birthDate < MinDate || birthDate > MaxDate)
                {
                    Console.WriteLine(message);
                    return false;
                }
                return true;
            }

        }
    }
}
