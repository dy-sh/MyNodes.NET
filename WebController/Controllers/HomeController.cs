using System.Web.Mvc;

namespace MyNetSensors.WebController.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("NodesView", "Gateway");
          //  return View();
        }


        public ActionResult Contact()
        {
            return View();
        }

    

    }
}