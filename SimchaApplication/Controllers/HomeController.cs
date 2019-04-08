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
            vm.Count = mgr.GetContributorCount();
            vm.Message = (string)TempData["Message"];
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
            vm.Message = (string)TempData["Message"];
            vm.Total = mgr.GetTotal();
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

        public ActionResult Contributions(int SimchaId)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            ContributionsViewModel vm = new ContributionsViewModel();
            vm.contributions = mgr.GetAllContributions(SimchaId);
            vm.SimchaName = mgr.GetSimchaNameById(SimchaId);
            vm.SimchaId = SimchaId;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddSimcha(Simcha simcha)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.InsertSimcha(simcha);
            TempData["Message"] = "The simcha was succesfully added!!";
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
            TempData["Message"] = "The contributor was succesfully added!!";
            return RedirectToAction("contributors", "home");
        }

        [HttpPost]
        public ActionResult AddDiposit(Diposit diposit)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.InsertDiposit(diposit);
            TempData["Message"] = "The diposit was succesfully added!!";
            return RedirectToAction("contributors", "home");
        }

        [HttpPost]
        public ActionResult EditContributor(Contributor contributor)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.UpdateContributor(contributor);
            TempData["Message"] = "The contributor was succesfully updated!!";
            return RedirectToAction("contributors", "home");
        }

        [HttpPost]
        public ActionResult UpdateContributions(List<ContributionView> cont, int SimchaId)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.DeleteContributions(SimchaId);
            IEnumerable<Contribution> contributions = cont.Where((s) => s.Contribute).Select((c) =>
            {
                return new Contribution
                {
                    SimchaId = SimchaId,
                    ContributorId = c.ContributorId,
                    Amount = c.Amount
                };
            });
            mgr.InsertContribution(contributions.ToList());
            TempData["Message"] = $"You have succesfully updated the {mgr.GetSimchaNameById(SimchaId)} simcha!!";
            return RedirectToAction("index", "home");
        }
    }
}