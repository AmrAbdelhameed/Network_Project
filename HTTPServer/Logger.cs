using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            FileStream fs = new FileStream("D:\\_FCIS\\Sna 4\\Semester 1\\Network\\Sections\\HTTP\\HTTP\\project\\Template[2018-2019]\\HTTPServer\\bin\\Debug\\log.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            // for each exception write its details associated with datetime
            sw.WriteLine(ex.Message, DateTime.Now);
            sw.Close();
        }
    }
}
