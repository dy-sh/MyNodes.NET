using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.ViewModels.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyNetSensors.WebController.Controllers
{
    public class ConfigController : Controller
    {

        private const string SETTINGS_FILE_NAME = "appsettings.json";

        public IActionResult Index()
        {
            return RedirectToAction("SerialPort");
        }



        [HttpGet]
        public IActionResult SerialPort()
        {
            List<string> ports = SerialController.comPort.GetPortsList();
            string currentPort = SerialController.serialPortName;

            var list = new List<SelectListItem>();
            foreach (string port in ports)
            {
                var item = new SelectListItem { Text = port, Value = port };
                if (port == currentPort)
                    item.Selected = true;

                list.Add(item);
            }

            ViewBag.ports = list;


            return View(new SerialPortViewModel());
        }


        [HttpPost]
        public IActionResult SerialPort(SerialPortViewModel port)
        {
            dynamic json = ReadConfig();
            json.SerialPort.Name = port.PortName;
            WriteConfig(json);

            SerialController.serialPortName = port.PortName;
            SerialController.ReconnectToSerialPort();


            return RedirectToAction("Messages", "Gateway");
        }

        private dynamic ReadConfig()
        {
            return JObject.Parse(System.IO.File.ReadAllText(SETTINGS_FILE_NAME));
        }

        private void WriteConfig(dynamic config)
        {
            System.IO.File.WriteAllText(SETTINGS_FILE_NAME, config.ToString());
        }
    }
}
