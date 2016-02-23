/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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

                return RedirectToAction("DatabaseCheck");
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

                            return RedirectToAction("DatabaseCheck");
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

        public IActionResult DatabaseCheck(string id)
        {
            //prevent start wizard if already passed
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);


            if (id == "Delete")
            {
                SystemController.ClearAllDatabases();
                return RedirectToAction("Gateway");
            }
            if (id == "Use")
            {
                return RedirectToAction("Gateway");
            }


            if ((SystemController.nodesDb.GetAllNodes() != null && SystemController.nodesDb.GetAllNodes().Count != 0)
                || (SystemController.usersDb.GetUsersCount() != 0))
                return View("DatabaseIsNotEmpty");

            return RedirectToAction("Gateway");
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
                User user = new User
                {
                    Name = "Admin",
                    Password = "Admin"
                };

                user.SetClaims(Users.User.GetAllClaims());

                await Authenticate(user);

                return View("UserProfileNoDatabase");
            }

            if (SystemController.usersDb.GetUsersCount() > 0)
                ViewBag.CanSkip = true;

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

            IUsersRepository db = SystemController.usersDb;

            if (ModelState.IsValid)
            {
                User user = db.GetUser(model.Name);
                if (user == null)
                {
                    user = new User()
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                    };

                    user.SetClaims(Users.User.GetAllClaims());

                    db.AddUser(user);

                    await Authenticate(user);

                    return RedirectToAction("Complete");
                }

                ModelState.AddModelError("", "User already exists");
            }
            return View(model);
        }



        public IActionResult Complete(RegisterModel model)
        {
            //prevent start wizard if already passed
            if (!bool.Parse(configuration["FirstRun"]))
                return View("Error", ALREADY_PASSED_MESSAGE);

            //redirect to first step if user came this url directly
            if (SystemController.dataBaseConfig == null)
                return RedirectToAction("Index");

            dynamic json = ReadConfig();
            json.FirstRun = false;
            WriteConfig(json);
            configuration.Reload();

            SystemController.Start(SystemController.configuration, SystemController.services);

            return RedirectToAction("Index", "Home");
        }



        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("Name", user.Name));

            if (user.GetClaims() != null)
                foreach (var claim in user.GetClaims())
                {
                    claims.Add(new Claim(claim, ""));
                }


            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));
        }



    }
}
