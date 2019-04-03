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
            IEnumerable<Contributor> contributors = mgr.GetAllContributors();
            vm.Contributors = contributors.Select((c) =>
            {
                return new ContributorViewModel
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    CellNumber = c.CellNumber,
                    AlwaysInclude = c.AlwaysInclude,
                    Date = c.Date,
                    Balance = mgr.GetBalance(c.Id)
                };
            });
            return View(vm);
        }

        public ActionResult History(int ContributorId)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            HistoryViewModel vm = new HistoryViewModel
            {
                Histories = mgr.GetHistory(ContributorId),
                Balance = mgr.GetBalance(ContributorId),
                Name = mgr.GetContributorNameById(ContributorId)
            };
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

        [HttpPost]
        public ActionResult AddDiposit(Diposit diposit)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.InsertDiposit(diposit);
            return RedirectToAction("contributors", "home");
        }

        [HttpPost]
        public ActionResult EditContributor(Contributor contributor)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.UpdateContributor(contributor);
            return RedirectToAction("contributors", "home");
        }
    }
}