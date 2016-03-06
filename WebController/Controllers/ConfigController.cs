/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using MyNodes.Users;
using MyNodes.WebController.Code;
using MyNodes.WebController.ViewModels.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyNodes.WebController.Controllers
{
    [Authorize(UserClaims.ConfigObserver)]
    public class ConfigController : Controller
    {

        private const string SETTINGS_FILE_NAME = "appsettings.json";
        private string settings_file;
        private string project_file;
        private IConfigurationRoot configuration;

        public ConfigController(IConfigurationRoot configuration, IApplicationEnvironment appEnv)
        {
            this.configuration = configuration;
            string applicationPath = appEnv.ApplicationBasePath;
            settings_file = Path.Combine(applicationPath, SETTINGS_FILE_NAME);
            project_file = Path.Combine(applicationPath, "project.json");
        }

        private dynamic ReadConfig()
        {
            return JObject.Parse(System.IO.File.ReadAllText(settings_file));
        }

        private void WriteConfig(dynamic config)
        {
            System.IO.File.WriteAllText(settings_file, config.ToString());
        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public IActionResult SerialGateway()
        {
            ViewBag.ports = SerialConnectionPort.GetAvailablePorts();

            return View(new SerialGatewayViewModel
            {
                PortName = SystemController.gatewayConfig.SerialGatewayConfig.SerialPortName,
                Boudrate = SystemController.gatewayConfig.SerialGatewayConfig.Boudrate
            });
        }


        [Authorize(UserClaims.ConfigEditor)]

        [HttpPost]
        public IActionResult SerialGateway(SerialGatewayViewModel model)
        {
            if (String.IsNullOrEmpty(model.PortName))
                return RedirectToAction("SerialGateway");

            dynamic json = ReadConfig();
            json.Gateway.SerialGateway.SerialPortName = model.PortName;
            json.Gateway.SerialGateway.Boudrate = model.Boudrate;
            WriteConfig(json);
            configuration.Reload();

            SystemController.DisconnectGateway();
            SystemController.gatewayConfig.SerialGatewayConfig.SerialPortName = model.PortName;
            SystemController.gatewayConfig.SerialGatewayConfig.Boudrate = model.Boudrate;

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult EthernetGateway()
        {
            EthernetGatewayViewModel model = new EthernetGatewayViewModel
            {
                Ip = SystemController.gatewayConfig.EthernetGatewayConfig.GatewayIP,
                Port = SystemController.gatewayConfig.EthernetGatewayConfig.GatewayPort
            };

            return View(model);
        }


        [Authorize(UserClaims.ConfigEditor)]

        [HttpPost]
        public IActionResult EthernetGateway(EthernetGatewayViewModel model)
        {
            dynamic json = ReadConfig();
            json.Gateway.EthernetGateway.GatewayIP = model.Ip;
            json.Gateway.EthernetGateway.GatewayPort = model.Port;
            WriteConfig(json);
            configuration.Reload();

            SystemController.DisconnectGateway();
            SystemController.gatewayConfig.EthernetGatewayConfig.GatewayIP = model.Ip;
            SystemController.gatewayConfig.EthernetGatewayConfig.GatewayPort = model.Port;

            return RedirectToAction("Index");
        }


        [Authorize(UserClaims.ConfigEditor)]

        public async Task<bool> ConnectSerialGateway()
        {
            SystemController.DisconnectGateway();

            dynamic json = ReadConfig();
            json.Gateway.SerialGateway.Enable = true;
            json.Gateway.EthernetGateway.Enable = false;
            WriteConfig(json);
            configuration.Reload();

            SystemController.gatewayConfig.SerialGatewayConfig.Enable = true;
            SystemController.gatewayConfig.EthernetGatewayConfig.Enable = false;

            await Task.Run((() =>
            {
                SystemController.ConnectToGateway();
            }));

            return true;
        }



        [Authorize(UserClaims.ConfigEditor)]

        public async Task<bool> ConnectEthernetGateway()
        {
            SystemController.DisconnectGateway();

            dynamic json = ReadConfig();
            json.Gateway.SerialGateway.Enable = false;
            json.Gateway.EthernetGateway.Enable = true;
            WriteConfig(json);
            configuration.Reload();

            SystemController.gatewayConfig.SerialGatewayConfig.Enable = false;
            SystemController.gatewayConfig.EthernetGatewayConfig.Enable = true;

            await Task.Run((() =>
            {
                SystemController.ConnectToGateway();
            }));

            return true;
        }


        [Authorize(UserClaims.ConfigEditor)]

        public bool DisconnectGateway()
        {
            dynamic json = ReadConfig();
            json.Gateway.SerialGateway.Enable = false;
            json.Gateway.EthernetGateway.Enable = false;
            WriteConfig(json);
            configuration.Reload();

            SystemController.gatewayConfig.SerialGatewayConfig.Enable = false;
            SystemController.gatewayConfig.EthernetGatewayConfig.Enable = false;

            SystemController.DisconnectGateway();

            return true;
        }


        [Authorize(UserClaims.ConfigEditor)]

        public bool StartNodesEngine()
        {
            dynamic json = ReadConfig();
            json.NodesEngine.Enable = true;
            WriteConfig(json);
            configuration.Reload();

            SystemController.nodesEngine.Start();

            return true;
        }



        [Authorize(UserClaims.ConfigEditor)]

        public bool StopNodesEngine()
        {
            dynamic json = ReadConfig();
            json.NodesEngine.Enable = false;
            WriteConfig(json);
            configuration.Reload();

            SystemController.nodesEngine.Stop();

            return true;
        }


        public WebServerInfo GetWebServerInfo()
        {
            if (SystemController.usersDb == null)
                return null;

            WebServerInfo info = new WebServerInfo();
            info.RegisteredUsersCount = SystemController.usersDb.GetUsersCount();

            return info;
        }



        [HttpGet]
        public IActionResult Rules()
        {
            return View(SystemController.webServerRules);
        }



        [Authorize(UserClaims.ConfigEditor)]

        [HttpPost]
        public IActionResult Rules(WebServerRules model)
        {
            if (model != null)
            {
                SystemController.webServerRules = model;

                dynamic json = ReadConfig();
                json.WebServer.Rules = JObject.FromObject(model);
                WriteConfig(json);
                configuration.Reload();
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Server()
        {
            return View(SystemController.webServerConfig);
        }



        [Authorize(UserClaims.ConfigEditor)]

        [HttpPost]
        public IActionResult Server(WebServerConfig model)
        {
            if (model != null)
            {
                SystemController.webServerConfig = model;

                dynamic json = ReadConfig();
                json.WebServer.Address = model.Address;
                WriteConfig(json);

                json= JObject.Parse(System.IO.File.ReadAllText(project_file));
                json.commands.web = $"Microsoft.AspNet.Server.Kestrel --server.urls {model.Address}";
                System.IO.File.WriteAllText(project_file, json.ToString());

                configuration.Reload();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult NodeEditor()
        {
            return View(SystemController.nodeEditorConfig);
        }


        [Authorize(UserClaims.ConfigEditor)]

        [HttpPost]
        public IActionResult NodeEditor(NodeEditorConfig model)
        {
            dynamic json = ReadConfig();
            json.NodeEditor = JObject.FromObject(model);
            WriteConfig(json);
            configuration.Reload();

            SystemController.nodeEditorConfig = model;

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult NodesEngine()
        {
            return View(SystemController.nodesEngineConfig);
        }


        [Authorize(UserClaims.ConfigEditor)]

        [HttpPost]
        public IActionResult NodesEngine(NodesEngineConfig model)
        {
            model.Enable = SystemController.nodesEngine.IsStarted();

            if (model.UpdateInterval < 0)
                model.UpdateInterval = 0;

            dynamic json = ReadConfig();
            json.NodesEngine = JObject.FromObject(model);
            WriteConfig(json);
            configuration.Reload();

            SystemController.nodesEngineConfig = model;
            SystemController.nodesEngine.SetUpdateInterval(model.UpdateInterval);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Console()
        {
            return View(SystemController.logs.consoleConfig);
        }


        [Authorize(UserClaims.ConfigEditor)]

        [HttpPost]
        public IActionResult Console(ConsoleConfig model)
        {

            dynamic json = ReadConfig();
            json.Console = JObject.FromObject(model);
            WriteConfig(json);
            configuration.Reload();

            SystemController.logs.consoleConfig = model;

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Logs()
        {
            return View(SystemController.logs.config);
        }


        [Authorize(UserClaims.ConfigEditor)]

        [HttpPost]
        public IActionResult Logs(LogsConfig model)
        {

            dynamic json = ReadConfig();
            json.Logs = JObject.FromObject(model);
            WriteConfig(json);
            configuration.Reload();

            SystemController.logs.config = model;

            return RedirectToAction("Index");
        }
    }

}
