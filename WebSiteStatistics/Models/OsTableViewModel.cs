using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteStatistics.Models
{
    public class OsTableViewModel
    {
        private string _osIcon;

        public string OsIcon
        {
            get { return _osIcon; }
            set
            {

                switch (value)
                {
                    case "Other":
                        _osIcon = "question-circle";
                        break;
                    case "iOS":
                        _osIcon = "apple";
                        break;
                    case "Mac OS X":
                        _osIcon = "apple";
                        break;
                    case "Mac OS":
                        _osIcon = "apple";
                        break;
                    case "Ubuntu":
                        _osIcon = "linux";
                        break;
                    default:
                        _osIcon = value;
                        break;
                }
            }
        }

        public string OsName { get; set; }
        public int OsViewCount { get; set; }
        public int TottalVisits { get; set; }

    }
}