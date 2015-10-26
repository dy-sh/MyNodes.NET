/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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
        private static string logFileName;

        static LogFile()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            logFileName =  String.Format(directory+"\\log_{0}.txt", DateTime.Now.ToString("MM.dd.yyyy HH_mm_ss"));
        }

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
