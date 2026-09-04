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

      
            if (Person.IsPersonExists(1))
            {
                Console.WriteLine("Could not find person");
            }
            else
            {
                Console.WriteLine($"Person Found");



            }
        }
    }
}
