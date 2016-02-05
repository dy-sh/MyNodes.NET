using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MyNetSensors.WebController.Code;
using MyNetSensors.WebController.ViewModels.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyNetSensors.WebController.Controllers
{
    [Authorize]
    public class ConfigController : Controller
    {

        private const string SETTINGS_FILE_NAME = "appsettings.json";
        private IConfigurationRoot configuration;

        public ConfigController(IConfigurationRoot configuration)
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
        public IActionResult SerialGateway()
        {
            ViewBag.ports = SerialConnectionPort.GetAvailablePorts();

            return View(new SerialGatewayViewModel
            {
                PortName = SystemController.gatewayConfig.SerialGatewayConfig.SerialPortName,
                Boudrate = SystemController.gatewayConfig.SerialGatewayConfig.Boudrate
            });
        }


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



        public bool StartNodesEngine()
        {
            dynamic json = ReadConfig();
            json.NodesEngine.Enable = true;
            WriteConfig(json);
            configuration.Reload();

            SystemController.nodesEngine.Start();

            return true;
        }


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

            WebServerInfo info=new WebServerInfo();
            info.RegisteredUsersCount = SystemController.usersDb.GetUsersCount();

            return info;
        }



        [HttpGet]
        public IActionResult Rules()
        {
            return View(SystemController.webServerRules);
        }


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


    }

}
