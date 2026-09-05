using System;


namespace Common
{
    public class Attributes
    {
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class NonNullableVariableAttribute:Attribute
        {
          
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class DateVariableAttribute:Attribute
        {

            public DateTime MinDate;
            public DateTime MaxDate;
            public DateVariableAttribute()
            {
                this.MaxDate = DateTime.Now.AddYears(-18);
                this.MinDate = DateTime.Now.AddYears(-65);
            }
        }
    }
}
