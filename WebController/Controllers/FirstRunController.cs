using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.Users;
using MyNetSensors.WebController.Code;
using MyNetSensors.WebController.ViewModels.Config;
using MyNetSensors.WebController.ViewModels.FirstRun;
using MyNetSensors.WebController.ViewModels.User;
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


        private string ALREADY_PASSED_MESSAGE =
            "First Run wizard has already been passed. To start it again, change \"FirstRun\" to \"true\" in appsettings.json file and restart server.";

        public IActionResult Index()
        {
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);


            return View();
        }

        [HttpGet]
        public IActionResult Database(string id)
        {
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);

            if (id == "None")
            {
                dynamic json = ReadConfig();
                json.DataBase.Enable = false;
                WriteConfig(json);
                configuration.Reload();

                SystemController.ReadConfig();
                SystemController.ConnectToDB();

                return RedirectToAction("Gateway");
            }
            if (id == "Builtin")
            {
                dynamic json = ReadConfig();
                json.DataBase.Enable = true;
                json.DataBase.UseInternalDb = true;
                WriteConfig(json);
                configuration.Reload();

                SystemController.ReadConfig();
                SystemController.ConnectToDB();

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
            //prevent start wizard if already passed
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);

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

                            SystemController.ReadConfig();
                            SystemController.ConnectToDB();

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
            //prevent start wizard if already passed
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);

            //redirect to first step if user came this url directly
            if (SystemController.dataBaseConfig == null)
                return RedirectToAction("Index");

            if (id == "None")
            {
                dynamic json = ReadConfig();
                json.Gateway.SerialGateway.Enable = false;
                json.Gateway.EthernetGateway.Enable = false;
                WriteConfig(json);
                configuration.Reload();

                SystemController.DisconnectGateway();
                SystemController.ReadConfig();
                SystemController.ConnectToGateway();

                return RedirectToAction("UserProfile");
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
            //prevent start wizard if already passed
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);

            //redirect to first step if user came this url directly
            if (SystemController.dataBaseConfig == null)
                return RedirectToAction("Index");

            dynamic json = ReadConfig();
            json.Gateway.SerialGateway.SerialPortName = model.PortName;
            json.Gateway.SerialGateway.Boudrate = model.Boudrate;
            json.Gateway.SerialGateway.Enable = true;
            json.Gateway.EthernetGateway.Enable = false;
            WriteConfig(json);
            configuration.Reload();

            SystemController.DisconnectGateway();
            SystemController.ReadConfig();
            SystemController.ConnectToGateway();

            return RedirectToAction("UserProfile");
        }

        [HttpPost]
        public IActionResult GatewayEthernet(EthernetGatewayViewModel model)
        {
            //prevent start wizard if already passed
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);

            //redirect to first step if user came this url directly
            if (SystemController.dataBaseConfig == null)
                return RedirectToAction("Index");

            dynamic json = ReadConfig();
            json.Gateway.EthernetGateway.GatewayIP = model.Ip;
            json.Gateway.EthernetGateway.GatewayPort = model.Port;
            json.Gateway.SerialGateway.Enable = false;
            json.Gateway.EthernetGateway.Enable = true;
            WriteConfig(json);
            configuration.Reload();

            SystemController.DisconnectGateway();
            SystemController.ReadConfig();
            SystemController.ConnectToGateway();

            return RedirectToAction("UserProfile");
        }



        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            //prevent start wizard if already passed
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);

            //redirect to first step if user came this url directly
            if (SystemController.dataBaseConfig == null)
                return RedirectToAction("Index");

            if (!SystemController.dataBaseConfig.Enable)
            {
                dynamic json = ReadConfig();
                json.FirstRun = false;
                WriteConfig(json);
                configuration.Reload();

                SystemController.Start(SystemController.configuration, SystemController.services);

                await Authenticate("Guest");

                return View("UserProfileNoDatabase");
            }

            return View(new RegisterModel());
        }


        [HttpPost]
        public async Task<IActionResult> UserProfile(RegisterModel model)
        {
            //prevent start wizard if already passed
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);

            //redirect to first step if user came this url directly
            if (SystemController.dataBaseConfig == null)
                return RedirectToAction("Index");

            IUsersRepository db = SystemController.usersRepository;

            if (ModelState.IsValid)
            {
                User user = db.GetUser(model.Name);
                if (user == null)
                {
                    db.AddUser(new User
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password
                    });

                    dynamic json = ReadConfig();
                    json.FirstRun = false;
                    WriteConfig(json);
                    configuration.Reload();

                    SystemController.Start(SystemController.configuration, SystemController.services);

                    await Authenticate(model.Name);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "User already exists");
            }
            return View(model);
        }


        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim> { new Claim("name", userName) };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return RedirectToAction("Login", "User");
        }

    }
}
