using System;


namespace Common
{

    /// <summary>
    /// Bu sınıf içinde, sistemde olmasını istediğimiz tüm attributlar alt sınıf olarak bulunur
    /// </summary>
    public class Attributes
    {


        [AttributeUsage(AttributeTargets.Property)]
        public abstract class PropertiesValidationAttribute : Attribute
        {
            public abstract bool IsValid(object value, string message);
        }


        [AttributeUsage(AttributeTargets.Property)]
        public class RequiredStringVariableAttribute : PropertiesValidationAttribute
        {
            public override bool IsValid(object value, string message)
            {
                if (value is string && !string.IsNullOrEmpty(value.ToString()))
                {

                    return true;
                }
                Console.WriteLine(message);
                return false;

            }
        }


        [AttributeUsage(AttributeTargets.Property)]
        public class PositiveIntegerAttribute : PropertiesValidationAttribute
        {
            public override bool IsValid(object value, string message)
            {

                if (!(value is int result))
                {

                    Console.WriteLine(message);
                    return false;
                }
                if (result <= 0)
                {
                    Console.WriteLine(message);
                    return false;

                }
                return true;
            }
        }




        [AttributeUsage(AttributeTargets.Property)]
        public class ValidateDateAttribute : PropertiesValidationAttribute
        {
            public DateTime MinDate;
            public DateTime MaxDate;


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
