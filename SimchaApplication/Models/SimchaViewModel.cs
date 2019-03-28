using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaData;

namespace SimchaApplication.Models
{
    public class SimchaViewModel
    {
        public IEnumerable<Simcha> Simchas { get; set; }
    }

    public class ContributorsViewModel
    {
        public IEnumerable<Contributor> Contributors { get; set; }
    }
}