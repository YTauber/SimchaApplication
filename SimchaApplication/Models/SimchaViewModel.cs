using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaData;

namespace SimchaApplication.Models
{
    public class SimchaViewModel
    {
        public IEnumerable<SimchaView> Simchas { get; set; }
        public int Count { get; set; }
        public string Message { get; set; }
    }

    public class ContributionsViewModel
    {
        public IEnumerable<ContributionView> contributions { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
    }

    public class ContributorsViewModel
    {
        public IEnumerable<ContributorViewModel> Contributors { get; set; }
        public decimal Total { get; set; }
        public string Message { get; set; }
    }

    public class HistoryViewModel
    {
        public IEnumerable<History> Histories { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

   public class ContributorViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public DateTime Date { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Balance { get; set; }
    }
}