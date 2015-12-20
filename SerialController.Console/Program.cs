/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/
using MyNetSensors.SerialController;

namespace MyNetSensors.SerialController.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            SerialController.OnDebugStateMessage += System.Console.WriteLine;
            SerialController.OnDebugTxRxMessage += System.Console.WriteLine;
            SerialController.Start("COM3");
            while (true)
                System.Console.ReadLine();
        }
    }
}
