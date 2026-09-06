
using System;
using System.Diagnostics;
using BCMS_Business;
using BCMS_Business.People;
using BCMS_Business.Products;
using BCMS_Business.Sessions;
using BCMS_Business.Staff;
using BCMS_Business.Tables;
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


            //User user = User.Find(2);
            //user.UserName = "ibra";
            //user.IsActive = true;

            //user.PersonID = 1;
            //user.Save();

            //Staff staff = new Staff();
            //staff.HireDate = DateTime.Now;
            //staff.Salary = 12311;
            //staff.PersonID = 1;
            //staff.CreatedByUserID = 1;
            //staff.StillWorking = true;


            //if(staff.Save())
            //{
            //    Console.WriteLine("Saved");
            //}


            //TablePrices price = new TablePrices();
            //price.PricePerHour = 11.7M;
            //price.CreatedByUserID = 1;
            //price.Description = "VIP";

            //if (price.Save())
            //    Console.WriteLine("Saved");

            //Table t = new Table();

            //t.TableStatus = Table.enTableStatus.Passive;
            //t.TableType = Table.enTableType.Caroom;
            //t.PriceID = 1;
            //t.Save();




        }

    }
}
