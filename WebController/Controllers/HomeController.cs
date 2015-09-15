using System.Web.Mvc;

namespace MyNetSensors.WebController.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("NodesControl", "Gateway");
          //  return View();
        }


        public ActionResult About()
        {
            return View();
        }

    

    }
}