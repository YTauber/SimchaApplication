using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaData;
using SimchaApplication.Models;

namespace SimchaApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            SimchaViewModel vm = new SimchaViewModel();
            vm.Simchas= mgr.GetAllSimchas();
            return View(vm);
        }

        public ActionResult Contributors()
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            ContributorsViewModel vm = new ContributorsViewModel();
            vm.Contributors = mgr.GetAllContributors();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddSimcha(Simcha simcha)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.InsertSimcha(simcha);
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public ActionResult AddContributor(Contributor contributor, decimal diposit)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.InsertContributor(contributor);
            mgr.InsertDiposit(new Diposit
            {
                Amount = diposit,
                ContributorId = contributor.Id,
                Date = contributor.Date
            });
            return RedirectToAction("contributors", "home");
        }
    }
}