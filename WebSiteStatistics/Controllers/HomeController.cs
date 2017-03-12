using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.WebPages;
using DataLayer.DbContext;
using DomainClasses.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UAParser;
using WebSiteStatistics.Models;

namespace WebSiteStatistics.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            IList<Statistics> stat = new List<Statistics>();
            using (var db = new AppDbContext())
            {
                stat = db.Statisticses.ToList();
            }

            StatisticsViewModel svm = new StatisticsViewModel()
            {
                //OnlineUsers = (int)HttpContext.Application["OnlineUsersCount"],
                TodayVisits = stat.Count(ss => ss.DateStamp.Day == DateTime.Now.Day),
                TotallVisits = stat.Count,
                UniquVisitors = stat.GroupBy(ta => ta.IpAddress).Select(ta => ta.Key).Count(),

            };


            return View(svm);
        }

        public int calculatePercentage(int CurrentValue, int totallValue)
        {

            return (int)CurrentValue * 100 / totallValue;

        }

        public ActionResult Table()
        {

            using (var db = new AppDbContext())
            {
                var countries = db.Countries.AsNoTracking().ToList();


                return View(countries);
            }
        }
        public ActionResult Chart()
        {
            IList<CountryViewModel> cvm = new List<CountryViewModel>();
            using (var db = new AppDbContext())
            {

                var countries = db.Countries.AsNoTracking().ToList();
                int totalvisits = countries.Sum(country => country.ViewCount);
                foreach (var country in countries)
                {

                    cvm.Add(new CountryViewModel()
                    {
                        ViewCount = country.ViewCount,
                        CountryName = country.CountryName,
                        TotalVisits = totalvisits,
                        Percentage = country.ViewCount * 100 / totalvisits

                    });
                }

                return View(cvm);
            }
        }
        public ActionResult Map()
        {
            return View();
        }

        [HttpGet]
        public JsonResult RequestMapData()
        {
            using (var db = new AppDbContext())
            {

                var countries = db.Countries.AsNoTracking().ToList();
                return Json(countries, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult RequestUserOsData()
        {
            using (var db = new AppDbContext())
            {
                var results = db.Statisticses.GroupBy(ua => new { ua.UserOs }).Select(g => new { lable = g.Key.UserOs, data = g.Count() }).ToArray();
                return Json(results, JsonRequestBehavior.AllowGet);
            }



        }
        [HttpGet]
        public JsonResult RequestUserBrowserData()
        {
            using (var db = new AppDbContext())
            {
                var results = db.Statisticses.GroupBy(ua => new { ua.UserAgent }).Select(g => new { lable = g.Key.UserAgent, value = g.Count() }).ToArray();
                return Json(results, JsonRequestBehavior.AllowGet);
            }



        }
        [HttpGet]
        public JsonResult RequestVisitorsCountryData()
        {
            using (var db = new AppDbContext())
            {

                var results = db.Countries.Select(c => new { y = c.CountryName, a = c.ViewCount }).ToArray();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult RequestVisitorsVectorMapData()
        {
            var countrydata = new List<VectorMapViewModel>();
            using (var db = new AppDbContext())
            {
                var results = db.Countries;
                countrydata.AddRange(results.Select(country => new VectorMapViewModel
                {
                    CountryCode = country.CountryCode,
                    CountryVisit = country.ViewCount
                }));
                var jd=JsonConvert.SerializeObject(countrydata);
                return Json(jd, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult VectorMap()
        {
            return View("VectorMap");
        }

        public ActionResult BlockeIps()
        {
            IList<BlockedIpViewModel> bivm = new List<BlockedIpViewModel>();
            using (var db = new AppDbContext())
            {
                var blockedips = db.BlockedIps.AsNoTracking().ToList();
                foreach (var blockedip in blockedips)
                {
                    bivm.Add(new BlockedIpViewModel()
                    {
                        Id = blockedip.Id,
                        IpAddress = blockedip.IpAddress
                    });

                }
                return View(bivm);

            }
        }
        [HttpPost]
        public ActionResult AddIp(string ipAddress)
        {
            var ips = new List<BlockedIpViewModel>();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                using (var db = new AppDbContext())
                {
                    var bi = new BlockedIp { IpAddress = ipAddress };
                    //db.BlockedIps.Add(bi);
                    //db.SaveChanges();
                    var ipss = db.BlockedIps.AsNoTracking().ToList();
                    ips.AddRange(ipss.Select(ipaddrss => new BlockedIpViewModel()
                    {
                        Id = ipaddrss.Id,
                        IpAddress = ipaddrss.IpAddress
                    }));
                }

            }
            return RedirectToAction("BlockeIps", ips);
        }

        public ActionResult DeleteIp(int Id)
        {
            var ips = new List<BlockedIpViewModel>();
            using (var db = new AppDbContext())
            {
                //db.BlockedIps.Remove(db.BlockedIps.First(i => i.Id == Id));
                //db.SaveChanges();
                var ipss = db.BlockedIps.AsNoTracking().ToList();
                ips.AddRange(ipss.Select(ipaddrss => new BlockedIpViewModel()
                {
                    Id = ipaddrss.Id,
                    IpAddress = ipaddrss.IpAddress
                }));
            }


            return RedirectToAction("BlockeIps", ips);


        }

        public ActionResult Chart2()
        {
            return View();
        }

        //بارگزاری اطلاعات برای جدول درصد استفاده از مرورگرها
        public ActionResult BrowserTable()
        {
            var btv = new List<BrowserTableViewModel>();
            using (var db = new AppDbContext())
            {
                var tottal = db.Statisticses.Count();
                btv.AddRange(db.Statisticses.GroupBy(ua => new { ua.UserAgent }).OrderByDescending(g => g.Count()).Select(g => new BrowserTableViewModel() {BrowserIcon =g.Key.UserAgent, BrowserName = g.Key.UserAgent , BrowserViewCount = g.Count(), TottalVisits = tottal }).ToList());

            }



            return PartialView("_BrowserTablePartial", btv);
        }

      
        //بارگزاری اطلاعات برای جدول درصد استفاده از سیستم عامل ها
        public ActionResult OsTable()
        {
            var otv = new List<OsTableViewModel>();
            using (var db = new AppDbContext())
            {
                var tottal = db.Statisticses.Count();
                otv.AddRange(db.Statisticses.GroupBy(ua => new { ua.UserOs }).OrderByDescending(g => g.Count()).Select(g => new OsTableViewModel() {OsIcon = g.Key.UserOs,OsName = g.Key.UserOs, OsViewCount = g.Count(), TottalVisits = tottal }).ToList());

            }



            return PartialView("_OsTablePartial", otv);
        }

        public ActionResult Referrer()
        {
            var ur = new List<ReferrerViewModel>();
            var st=new List<Statistics>();
            using (var db = new AppDbContext())
            {

                st = db.Statisticses.ToList();
            }
            foreach (var statisticse in st)
                {
                    statisticse.Referer = GetHostName(statisticse.Referer);
                }
                ur.AddRange(st.GroupBy(
                    r => new { r.Referer }).OrderByDescending(
                    r => r.Count()).Select(r => new ReferrerViewModel()
                    { ReferrerUrl = r.Key.Referer, ReferrerCount = r.Count() }).ToList());
                //r.Key.Referer.Substring(0, 100)
            

            return PartialView("_UserReferrerPartial", ur);
        }

        public string GetHostName(string url)
        {
            if (url != "Direct")
            {
                Uri uri=new Uri(url);
                return uri.Host;
            }
            else
            {
                return url;
            }
            
        }

        public ActionResult PageView()
        {
            var pv = new List<PageViewViewModel>();
            var st = new List<Statistics>();
            using (var db = new AppDbContext())
            {
                st = db.Statisticses.ToList();
            }
            foreach (var statisticse in st)
            {
                statisticse.PageViewed = NormalizePageName(statisticse.PageViewed);
            }

            pv.AddRange(st.GroupBy(
                   r => new { r.PageViewed }).OrderByDescending(
                   r => r.Count()).Select(r => new PageViewViewModel()
                   { PageUrl = r.Key.PageViewed, PageViewCount = r.Count() }).ToList());



            return PartialView("_PageViewPartial",pv);
        }


        /// <summary>
        /// متدی برای نرمال سازی نام صفحات
        /// </summary>
        /// <param name="PageName">نام صفحه بازدید شده</param>
        /// <returns>نرمال شده نام صفحه</returns>
        public string NormalizePageName(string PageName)
        {
            if (PageName == "/")
            {
                return "Home/Index";
            }
            else
            {
                return PageName.Remove(0,1);
            }


        }


        public ActionResult CurrentVisitor()
        {
            var Cv = new CurrentVisitorViewModel()
            {

                IpAddress = GetIPAddress(),
                Browser = Request.Browser.Browser,
                OsName = GetUserOS(Request.UserAgent)
            };



            return PartialView("_CurrentVisitorPartial", Cv);
        }

        public ActionResult Subdetails()
        {
            IList<Statistics> stat = new List<Statistics>();
            using (var db = new AppDbContext())
            {
                stat = db.Statisticses.AsNoTracking().ToList();
                
            }

            var subdetails=new SubDetailsViewModel
            {

                Today = stat.Count(d => d.DateStamp.Day == DateTime.Now.Day),
                LastDay = stat.Count(d => d.DateStamp.Day == DateTime.Now.Day-1),
                ThisMonth = stat.Count(m => m.DateStamp.Month == DateTime.Now.Month),
                ThisYear = stat.Count(y=>y.DateStamp.Year==DateTime.Now.Year),
                PeakDate = stat.GroupBy(x => x.DateStamp.ToShortDateString()).OrderByDescending(grouping => grouping.Count()).First().Key.AsDateTime(),
                LowDate = stat.GroupBy(x => x.DateStamp.ToShortDateString()).OrderByDescending(grouping => grouping.Count()).Last().Key.AsDateTime(),



            };

            //MostVisitedDate();

            return PartialView("_SubDetailsPartial",subdetails);
        }

     


        //Helpers
        public static string GetUserOS(string userAgent)
        {
            // get a parser with the embedded regex patterns
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            return c.OS.Family;
        }
        public string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }



    }

}