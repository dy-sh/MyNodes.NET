using System.Web.Mvc;

namespace MyNetSensors.WebServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Control", "Gateway");
          //  return View();
        }


        public ActionResult About()
        {
            return View();
        }

    

    }
}