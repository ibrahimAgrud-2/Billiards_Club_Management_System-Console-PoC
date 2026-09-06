
using System;
using System.Diagnostics;
using BCMS_Business;
using BCMS_Business.People;
using BCMS_Business.Users;
using Common;

namespace BCMS_ConsoleApp
{
    
    class program
    {
 
        static void Main()
        {
            
            Logger.SetLogAction(CommonTools.LogToConsole);
            //Logger.SetLogAction(CommonTools.LogToWindowsEventView);
            //Logger.SetLogAction(BCMS_Business.LogMessages.Logs.LogToDatabase);

            //Person p = new Person();
            //p.FirstName = "ibo";
            //p.LastName = "agr";
            //p.Address = "ist";
            //p.ImagePath = "C";
            //p.BirthDate = DateTime.Now.AddYears(-19);
            //p.Email = "ib@gma";
            //p.Phone = "53451231";


            User user = User.Find(2);
            user.UserName = "ibra";
            user.IsActive = true;

            user.PersonID = 1;
            user.Save();


            DateTime? dt = null;


        }

    }
}
