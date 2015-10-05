/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.WebController.Code;


namespace MyNetSensors.WebController.Controllers
{
    public class GatewayController : Controller
    {


        IHubContext clientsHub = GlobalHost.ConnectionManager.GetHubContext<ClientsHub>();




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



        public async Task<ActionResult> DropNodes()
        {
            await DropHistoryDatabase();
            string clientId = "";   //todo get client id
            GatewayClientStatic.gatewayClient.ClearNodes(clientId);

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
            GatewayClientStatic.gatewayClient.UpdateSensorsTasks(clientId);

        }

        private async Task DropTasksDatabase()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            ISensorsTasksRepository db = new SensorsTasksRepositoryDapper(cs);
            db.DropAllTasks();

            string clientId = "";   //todo get client id
            GatewayClientStatic.gatewayClient.UpdateSensorsTasks(clientId);
        }

        private async Task DropLinksDatabase()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            ISensorsLinksRepository db = new SensorsLinksRepositoryDapper(cs);
            db.DropAllLinks();

            string clientId = "";   //todo get client id
            GatewayClientStatic.gatewayClient.UpdateSensorsLinks(clientId);
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
                GatewayClientStatic.gatewayClient.UpdateNodeSettings(clientId, node);
                await Task.Delay(100);
            }
        }



    }
}