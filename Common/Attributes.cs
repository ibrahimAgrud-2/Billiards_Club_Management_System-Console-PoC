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
    }
}
