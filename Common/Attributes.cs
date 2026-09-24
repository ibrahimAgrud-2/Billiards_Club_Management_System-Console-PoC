using System;


namespace Common
{

    /// <summary>
    /// Bu sınıf içinde, sistemde olmasını istediğimiz tüm attributlar alt sınıf olarak bulunur
    /// </summary>
    public class Attributes
    {


        /*
    * biz PropertiesValidationAttribute'i yani ana attribute'ı şu yüzden tanımladık: her bir attribute türünü ayrı ayrı kontrol etmek yerine bir ana attribute'u kontrol ederiz. O ana attribute alt attributeların isValid fonksyionlarını çağıracak.
    */
        [AttributeUsage(AttributeTargets.Property)]
        public abstract class PropertiesValidationAttribute : Attribute
        {
            public abstract bool IsValid(object value, string message);
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class RequiredVariableAttribute : PropertiesValidationAttribute
        {
            public override bool IsValid(object value, string message)
            {
                if (value is string && string.IsNullOrEmpty(value.ToString()))
                {
                    Console.WriteLine(message);
                    return false;
                }
                if (value == null)
                {
                    Console.WriteLine(message);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// ilk paramtere bu günden itibaren ne kadar geride olabilir. Mesela -65 bugünden 65 yıl önce demek. Yani 1964 gibi. İkinci paramtere ise en geç kaç olabilr. 
        /// (-65,0) demek min 1964 max bugün
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class ValidateDateAttribute : PropertiesValidationAttribute
        {
            public DateTime MinDate;
            public DateTime MaxDate;

            /*
             paramtereler int olacak. minDate ve MaxDate şekilden (daha nantıklı bir paramtere ismi bulunabilir)
            Şu şekilde çalışacak:

                BirthDate için ben (-65,-18) dedim bu demek oluyor ki. min tarih günümüzden -65 daha uzakta yani 1964 civarı. ve max 2008 civarında olacak. Bu şekide kontrol edilebilir. Contructor içinde de şu şekilde ekleme yapıyorum this.MinDate = DateTime.Now.AddYears(minDate); this.MaxDate = DateTime.Now.AddYears(maxDate);
                HireDate için (-65,0) şeklinde olur. Bu sayede eski elemanları kayıt edebilir ama gelecekte eleman kayıt edemezsin. Yani bir eleman kayıt ederken tarihi yarın veremezsin
                SessionDate içinde (0,0) olarak veririm. Bu sayede ne dün ne de yarın için date kayıt edemezsin
            */
            public ValidateDateAttribute(int minDate, int maxDate)
            {
                this.MinDate = DateTime.Now.Date.AddYears(minDate);
                this.MaxDate = DateTime.Now.Date.AddYears(maxDate);
            }
            public override bool IsValid(object value, string message)
            {

                DateTime date = Convert.ToDateTime(value);
                if (date < MinDate)
                {
                    Console.WriteLine(message);
                    return false;
                }
                if (date > MaxDate)
                {
                    Console.WriteLine(message);
                    return false;
                }


                return true;

            }

        }


   


    }
}
