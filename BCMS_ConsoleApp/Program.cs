using BCMS_Business.People;
using System;
namespace BCMS_ConsoleApp
{
    
    class program
    {

    static void Main()
        {//Make coding faster AI 

            Person p = new Person();


          

            if(p.Save())
            {
                Console.WriteLine("YES");
            }


        }
       
    }
}
