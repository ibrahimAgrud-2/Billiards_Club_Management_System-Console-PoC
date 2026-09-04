using BCMS_Business;
using BCMS_Business.People;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace BCMS_ConsoleApp
{
    internal class Program
    {








        static void Main(string[] args)
        {
            //Layer layer adım adım gidelim.
            //Person DL ve BL Biraz uzun sürer ama diğerleri hep copy paster zaten
            //DeepSeak kullan.


            Person p1 = Person.Find(4);
            p1.FirstName = "ismail";
            p1.LastName = "Agr";
            p1.Email = "ismail@gmail";
            p1.BirthDate = DateTime.Today;
            p1.Address = "ist";
            p1.ImagePath = "C";
            p1.Phone = "0531231123";

            if (p1.DeletePerson())
            {
                Console.WriteLine("Person Deleted Successfully");
            }


        }
    }
}
