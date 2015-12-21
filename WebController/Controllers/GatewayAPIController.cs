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



    }
}
