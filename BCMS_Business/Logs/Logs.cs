using BCMS_Data.Logs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMS_Business.Logs
{
    public class Logs
    {
         
        public static DataTable GetLogs()
        {
            return LogsDataAccess.GetLogs();
        }

    }
}
