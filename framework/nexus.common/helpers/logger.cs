using System;
using System.Data;
using System.Globalization;
using System.IO;

namespace nexus.common

{
    static public partial class helpers
    {
        public static void WriteToLog(string Message)
        {
            if (!Directory.Exists(@"C:\Nexus\logging")) Directory.CreateDirectory(@"C:\Nexus\logging");
            string LogFile = "C:\\Nexus\\logging\\nexus_api_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            using (StreamWriter w = System.IO.File.AppendText(LogFile)) w.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "|| " + Message);
        }

    }

}
