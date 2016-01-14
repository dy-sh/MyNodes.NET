/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateways;
using MyNetSensors.SerialControllers;

namespace MyNetSensors.WebController.Controllers
{
    [ResponseCache(Duration = 0)]

    public class GatewayAPIController : Controller
    {
        private Gateway gateway = SerialController.gateway;


        public List<Node> GetNodes()
        {
            return gateway.GetNodes();
        }

        public bool IsHardwareConnected()
        {
            if (gateway == null)
                return false;

            return gateway.IsConnected();
        }

        //public string GetMessages()
        //{
        //    List<Message> messages = gateway.messagesLog.GetAllMessages();
        //    string text = null;
        //    foreach (var message in messages)
        //    {
        //        text += message.ToString();
        //        text += " <br>\n";
        //    }
        //    return text;
        //}

        //public bool ClearMessages()
        //{
        //    gateway.messagesLog.ClearLog();
        //    return true;
        //}

        public bool SendMessage(int nodeId, int sensorId, string state)
        {
            if (!gateway.IsConnected())
                return false;

            gateway.SendSensorState(nodeId, sensorId, state);
            return true;
        }




        public GatewayInfo GetGatewayInfo()
        {
            return gateway.GetGatewayInfo();
        }



        public bool UpdateNodeSettings(Node node)
        {
            gateway.UpdateNodeSettings(node);
            return true;
        }






        public bool UpdateNodesTasks()
        {
            SerialController.nodesTasksEngine.GetTasksFromRepository();
            return true;
        }

        public bool RemoveNode(int nodeId)
        {
            if (gateway.GetNode(nodeId) == null)
                return false;
            gateway.RemoveNode(nodeId);
            return true;
        }


        public async Task<bool> RemoveAllNodes()
        {
            await ClearHistory();

            gateway.RemoveAllNodes();

            return true;
        }






  

        public async Task<ActionResult> ClearHistory()
        {
            await StopWritingHistory();
            //waiting for all history writings finished
            await Task.Delay(2000);

            SerialController.historyDb.ClearAllHistory();

            return Json(true);

        }


        public ActionResult DisableTasks()
        {
            SerialController.nodesTasksDb.DisableTasks();
            
            UpdateNodesTasks();

             return Json(true);
        }

        public async Task<ActionResult> RemoveAllTasks()
        {
            DisableTasks();
            await Task.Delay(1000);
            SerialController.nodesTasksDb.RemoveAllTasks();

            UpdateNodesTasks();
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
