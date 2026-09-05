
using System;
using System.Diagnostics;
using BCMS_Business;
using Common;

namespace BCMS_ConsoleApp
{
    
    class program
    {
        static void Main()
        {
            //Log'ın nereye yazılacağını belirliyoruz.
            Logger.SetLogAction(CommonTools.LogToConsole);
            //Logger.SetLogAction(CommonTools.LogToWindowsEventView);
            //Logger.SetLogAction(BCMS_Business.Logs.Logs.LogToDatabase);




        }

    }
}
