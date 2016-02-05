using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using MyNetSensors.WebController.Code;
using MyNetSensors.WebController.ViewModels.Config;
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
            if (id == "None")
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
                        ModelState.AddModelError("", "Unable to connect to the database. " + ex.Message);
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




        [HttpGet]
        public IActionResult Gateway(string id)
        {
            if (id == "None")
            {
                dynamic json = ReadConfig();
                json.Gateway.SerialGateway.Enable = false;
                json.Gateway.EthernetGateway.Enable = false;
                WriteConfig(json);
                configuration.Reload();

                return RedirectToAction("User");
            }
            if (id == "Serial")
            {
                ViewBag.ports = SerialConnectionPort.GetAvailablePorts();

                return View("GatewaySerial", new SerialGatewayViewModel
                {
                    PortName = configuration["Gateway:SerialGateway:SerialPortName"],
                    Boudrate = Int32.Parse(configuration["Gateway:SerialGateway:Boudrate"])
                });
            }
            if (id == "Ethernet")
            {
                return View("GatewayEthernet", new EthernetGatewayViewModel
                {
                    Ip = configuration["Gateway:EthernetGateway:GatewayIP"],
                    Port = Int32.Parse(configuration["Gateway:EthernetGateway:GatewayPort"])
                });
            }

            return View();
        }

        [HttpPost]
        public IActionResult GatewaySerial(SerialGatewayViewModel model)
        {
            if (String.IsNullOrEmpty(model.PortName))
                return View("GatewaySerial", new SerialGatewayViewModel());

            dynamic json = ReadConfig();
            json.Gateway.SerialGateway.SerialPortName = model.PortName;
            json.Gateway.SerialGateway.Boudrate = model.Boudrate;
            WriteConfig(json);
            configuration.Reload();

            return RedirectToAction("User");
        }

        [HttpPost]
        public IActionResult GatewayEthernet(EthernetGatewayViewModel model)
        {
            dynamic json = ReadConfig();
            json.Gateway.EthernetGateway.GatewayIP = model.Ip;
            json.Gateway.EthernetGateway.GatewayPort = model.Port;
            WriteConfig(json);
            configuration.Reload();

            return RedirectToAction("User");
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
