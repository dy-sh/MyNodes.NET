using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using MyNetSensors.WebController.ViewModels.FirstRun;
using Newtonsoft.Json.Linq;

namespace MyNetSensors.WebController.Controllers
{
    public class FirstRunController : Controller
    {
        private const string SETTINGS_FILE_NAME = "appsettings.json";
        private IConfigurationRoot configuration;

        public FirstRunController(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
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

        [HttpGet]
        public IActionResult Database(string id)
        {
            if (id == "Non")
            {
                dynamic json = ReadConfig();
                json.DataBase.Enable = false;
                WriteConfig(json);
                configuration.Reload();

                return RedirectToAction("Gateway");
            }
            if (id == "Builtin")
            {
                dynamic json = ReadConfig();
                json.DataBase.Enable = true;
                json.DataBase.UseInternalDb = true;
                WriteConfig(json);
                configuration.Reload();

                return RedirectToAction("Gateway");
            }
            if (id == "External")
            {
                return View("DatabaseExternal", new ExternalDatabaseViewModel
                {
                    ConnectionString = configuration["DataBase:ExternalDbConnectionString"]
                });
            }

            return View();
        }

        [HttpPost]
        public IActionResult Database(ExternalDatabaseViewModel model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(model.ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            dynamic json = ReadConfig();
                            json.DataBase.Enable = true;
                            json.DataBase.UseInternalDb = false;
                            json.DataBase.ExternalDbConnectionString = model.ConnectionString;
                            WriteConfig(json);
                            configuration.Reload();
                            return View("DatabaseOK");
                        }
                        else
                        {
                            ModelState.AddModelError("",
                                "Unable to connect to the database. Check the connection string.");
                            return View("DatabaseExternal", model);
                        }
                    }
                    catch (SqlException ex)
                    {
                        ModelState.AddModelError("", "Unable to connect to the database. "+ ex.Message);
                        return View("DatabaseExternal", model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to connect to the database. " + ex.Message);
                return View("DatabaseExternal", model);
            }
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
        //    configuration.Reload();

        //    return RedirectToAction("Index", "Dashboard");
        //}

        [HttpGet]
        public IActionResult SerialPort()
        {


            return View();
        }
    }
}
