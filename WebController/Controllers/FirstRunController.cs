using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MyNetSensors.WebController.Controllers
{
    public class FirstRunController : Controller
    {
        private const string SETTINGS_FILE_NAME = "appsettings.json";
        private IConfigurationRoot сonfiguration;

        public FirstRunController(IConfigurationRoot сonfiguration)
        {
            this.сonfiguration = сonfiguration;
        }

        private dynamic ReadConfig()
        {
            return JObject.Parse(System.IO.File.ReadAllText(SETTINGS_FILE_NAME));
        }

        private void WriteConfig(dynamic config)
        {
            System.IO.File.WriteAllText(SETTINGS_FILE_NAME, config.ToString());
        }



        public IActionResult Index()
        {


            return View();
        }

        public IActionResult Database(string id)
        {
            if (id == "Non")
            {
                dynamic json = ReadConfig();
                json.DataBase.Enable = false;
                WriteConfig(json);
                сonfiguration.Reload();

                return RedirectToAction("Gateway");
            }
            if (id == "Builtin")
            {
                dynamic json = ReadConfig();
                json.DataBase.Enable = true;
                json.DataBase.UseInternalDb = true;
                WriteConfig(json);
                сonfiguration.Reload();

                return RedirectToAction("Gateway");
            }
            if (id == "External")
            {
                return View("DatabaseExternal");
            }

            return View();
        }

        public IActionResult Gateway(string id)
        {


            return View();
        }


        //[HttpPost]
        //public IActionResult Index()
        //{

        //    dynamic json = ReadConfig();
        //    json.FirstRun = false;
        //    WriteConfig(json);
        //    сonfiguration.Reload();

        //    return RedirectToAction("Index", "Dashboard");
        //}

        [HttpGet]
        public IActionResult SerialPort()
        {


            return View();
        }
    }
}
