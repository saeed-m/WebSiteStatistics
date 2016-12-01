using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteStatistics.Models
{
    public class CurrentVisitorViewModel
    {
        public string Browser { get; set; }
        public string IpAddress { get; set; }
        public string CountryName { get; set; }
        public string OsName { get; set; }
        


    }
}