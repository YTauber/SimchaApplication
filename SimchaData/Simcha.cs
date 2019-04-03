using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaData
{
    public class Simcha
    {
        public int Id { get; set; }
        public string SimchaName { get; set; }
        public DateTime Date { get; set; }
    }

    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public DateTime Date { get; set; }
        public bool AlwaysInclude { get; set; }
    }

    public class Contribution
    {
        public int SimchaId { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class Diposit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int ContributorId { get; set; }
    }

    public class History
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
