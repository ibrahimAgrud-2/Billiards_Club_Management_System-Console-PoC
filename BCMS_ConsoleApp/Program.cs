using BCMS_Business.OrderItems;
using BCMS_Business.People;
using BCMS_Business.Staff;
using BCMS_Business.Users;
using System;
using System.Runtime.CompilerServices;
using static Common.Attributes;
namespace BCMS_ConsoleApp
{
    
    class program
    {  


        static void Main()
        {//Make coding faster AI 


            Staff u = new Staff();
            u.FirstName = "ibra";
            u.LastName = "aa";
            u.Address = "add";
            u.ImagePath = "C";
            u.Email = "ibra@gmail.com";
            u.BirthDate = DateTime.Now.Date.AddYears(-18);
            u.Phone = "052342421";
            u.SeparationDate = null;
            u.HireDate = DateTime.Now.AddDays(-13);
            u.StillWorking = true;
            u.CreatedByUserID = 1;
        
       

            if (u.Save())
            {
                Console.WriteLine("yes");
            }
        }
       
    }
}
