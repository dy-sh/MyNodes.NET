/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.SerialControl;
using MyNetSensors.WebServer.Code;


namespace MyNetSensors.WebServer.Controllers
{

    public class GatewayController : Controller
    {
        private SerialGateway gateway = SerialController.gateway;

        public ActionResult Index()
        {
            return RedirectToAction("Control");
        }

        public ActionResult Messages()
        {
            return View();
        }


        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult Observe()
        {
            return View();
        }

        public ActionResult Control()
        {
            return View();
        }









        public ActionResult GetNodes()
        {
            List<Node> nodes = gateway.GetNodes();
            return Json(nodes);
        }

        public ActionResult IsHardwareConnected()
        {
            bool connected = gateway.IsConnected();
            return Json(connected);
        }

        public ActionResult GetMessages()
        {
            List<Message> messages = gateway.messagesLog.GetAllMessages();
            string text=null;
            foreach (var message in messages)
            {
                text += message.ToString();
                text += " <br>\n";
            }
            return Content(text);
        }

     


        public void ClearMessages()
        {
            gateway.messagesLog.ClearLog();
        }

        public void SendMessage(string message)
        {
            if (!gateway.IsConnected()) return;

            Message mess = gateway.ParseMessageFromString(message);
            gateway.SendMessage(mess);
        }


        
        public async Task<IActionResult> Test()
        {
            string s=null;
            for (Int64 i = 0; i < 100000; i++)
            {
                s += "7";
            }
            return Content(DateTime.Now.ToString());
        }




        public async Task<ActionResult> DropNodes()
        {
            await DropHistoryDatabase();
            string clientId = "";   //todo get client id
        //    GatewayClientStatic.gatewayClient.ClearNodes(clientId);

            return RedirectToAction("Settings");
        }

        public async Task<ActionResult> DropHistory()
        {
            await DropHistoryDatabase();

            return RedirectToAction("Settings");
        }

        public async Task<ActionResult> DropLinks()
        {
            await DropLinksDatabase();

            return RedirectToAction("Settings");
        }


        public async Task<ActionResult> DropTasks()
        {
            await DisableAllTasks();
            await Task.Delay(1000);
            await DropTasksDatabase();

            return RedirectToAction("Settings");
        }

        public async Task<ActionResult> DisableTasks()
        {
            await DisableAllTasks();

            return RedirectToAction("Settings");
        }

        public async Task<ActionResult> StopRecordingHistory()
        {
            await StopRecordingNodesHistory();

            return RedirectToAction("Settings");
        }

        private async Task DropHistoryDatabase()
        {
            await StopRecordingNodesHistory();
            //waiting for all history writings finished
            await Task.Delay(2000);

            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            ISensorsHistoryRepository db = new SensorsHistoryRepositoryDapper(cs);
            db.DropAllSensorsHistory();


            //todo check dropped
        }


        private async Task DisableAllTasks()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            ISensorsTasksRepository db = new SensorsTasksRepositoryDapper(cs);
            db.DisableAllTasks();

            string clientId = "";   //todo get client id
        //    GatewayClientStatic.gatewayClient.UpdateSensorsTasks(clientId);

        }

        private async Task DropTasksDatabase()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            ISensorsTasksRepository db = new SensorsTasksRepositoryDapper(cs);
            db.DropAllTasks();

            string clientId = "";   //todo get client id
         //   GatewayClientStatic.gatewayClient.UpdateSensorsTasks(clientId);
        }

        private async Task DropLinksDatabase()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            ISensorsLinksRepository db = new SensorsLinksRepositoryDapper(cs);
            db.DropAllLinks();

            string clientId = "";   //todo get client id
         //   GatewayClientStatic.gatewayClient.UpdateSensorsLinks(clientId);
        }

        private async Task StopRecordingNodesHistory()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            IGatewayRepository gatewayDb = new GatewayRepositoryDapper(cs);

            var nodes = gatewayDb.GetNodes();
            //turn off writing history in nodes settings
            foreach (var node in nodes)
            {
                foreach (var sensor in node.sensors)
                {
                    sensor.storeHistoryEnabled = false;
                }

                gatewayDb.UpdateNodeSettings(node);

                string clientId = "";   //todo get client id
            //    GatewayClientStatic.gatewayClient.UpdateNodeSettings(clientId, node);
                await Task.Delay(100);
            }
        }



    }
}