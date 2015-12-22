using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateway;
using MyNetSensors.SerialControl;

namespace MyNetSensors.WebServer.Controllers
{
    [ResponseCache(Duration = 0)]

    public class GatewayAPIController : Controller
    {
        private SerialGateway gateway = SerialController.gateway;


        public ActionResult GetNodes()
        {
            return Json(gateway.GetNodes());
        }

        public ActionResult IsHardwareConnected()
        {
            if (gateway == null)
                return Json(false);

            return Json(gateway.IsConnected());
        }

        public ActionResult GetMessages()
        {
            List<Message> messages = gateway.messagesLog.GetAllMessages();
            string text = null;
            foreach (var message in messages)
            {
                text += message.ToString();
                text += " <br>\n";
            }
            return Content(text);
        }

        public ActionResult ClearMessages()
        {
            gateway.messagesLog.ClearLog();
            return Json(true);
        }

        public ActionResult SendMessage(string message)
        {
            if (!gateway.IsConnected())
                return Json(false);

            Message mess = gateway.ParseMessageFromString(message);
            gateway.SendMessage(mess);
            return Json(true);
        }




        public ActionResult GetGatewayInfo()
        {
            GatewayInfo info = gateway.GetGatewayInfo();
            return Json(info);
        }



        public ActionResult UpdateNodeSettings(Node node)
        {
            gateway.UpdateNodeSettings(node);
            return Json(true);
        }


        public ActionResult ClearNodes()
        {
            gateway.ClearNodesList();
            return Json(true);
        }


        public ActionResult UpdateSensorsLinks()
        {
            SerialController.sensorsLinksEngine.GetLinksFromRepository();
            return Json(true);
        }

        public ActionResult UpdateSensorsTasks()
        {
            SerialController.sensorsTasksEngine.GetTasksFromRepository();
            return Json(true);
        }

        public ActionResult DeleteNode(int nodeId)
        {
            if (gateway.GetNode(nodeId) == null)
                return Json(false);
            gateway.DeleteNode(nodeId);
            return Json(true);
        }


        public async Task<ActionResult> DropNodes()
        {
            await DropHistory();

            ClearNodes();

            return Json(true);
        }






  

        public async Task<ActionResult> DropHistory()
        {
            await StopWritingHistory();
            //waiting for all history writings finished
            await Task.Delay(2000);

            SerialController.historyDb.DropHistory();

            return Json(true);

        }


        public ActionResult DisableTasks()
        {
            SerialController.sensorsTasksDb.DisableTasks();
            
            UpdateSensorsTasks();

             return Json(true);
        }

        public async Task<ActionResult> DropTasks()
        {
            DisableTasks();
            await Task.Delay(1000);
            SerialController.sensorsTasksDb.DropTasks();

            UpdateSensorsTasks();
             return Json(true);
        }

        public ActionResult DropLinks()
        {

            SerialController.sensorsLinksDb.DropLinks();

            UpdateSensorsLinks();
             return Json(true);
        }

        public async Task<ActionResult> StopWritingHistory()
        {
            var nodes = gateway.GetNodes();
            //turn off writing history in nodes settings
            foreach (var node in nodes)
            {
                foreach (var sensor in node.sensors)
                {
                    sensor.storeHistoryEnabled = false;
                }

                UpdateNodeSettings(node);

                await Task.Delay(100);
            }
             return Json(true);
        }

    }
}
