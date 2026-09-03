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


        public static DataTable GetPersonList()
        {
            return PersonDataAccess.GetPeople();
        }

    }
}
