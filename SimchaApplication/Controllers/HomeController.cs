using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaData;

namespace SimchaApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.GetAllSimchas();
            mgr.GetAllContributors();
            mgr.GetAllContributions();
            mgr.GetDipositsById(0);
            return View();
        }
    }
}