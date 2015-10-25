using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.SerialController.Service
{
    public static class LogFile
    {
        private static string logFileName = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";

        public static void WriteMessage(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                string mes = String.Format("{0} : EXCEPTION in {1} : {2}",
                    DateTime.Now,
                     ex.Source,
                     ex.Message);

                sw = new StreamWriter(logFileName, true);
                sw.WriteLine(mes);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }

        public static void WriteMessage(string message)
        {
            StreamWriter sw = null;
            try
            {
                string mes = String.Format("{0} : {1}",
                    DateTime.Now,
                    message);

                sw = new StreamWriter(logFileName, true);
                sw.WriteLine(mes);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }
}
