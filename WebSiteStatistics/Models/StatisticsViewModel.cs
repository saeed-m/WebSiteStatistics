using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteStatistics.Models
{
    public class StatisticsViewModel
    {
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string UserOs { get; set; }
        public string Referer { get; set; }
        public string PageViewed { get; set; }
        public DateTime DateStamp { get; set; }
        public int OnlineUsers { get; set; }
        public int TodayVisits { get; set; }
        public int YesterdayVisits { get; set; }
        public int TotallVisits { get; set; }
        public int UniquVisitors { get; set; }


        




    }
}