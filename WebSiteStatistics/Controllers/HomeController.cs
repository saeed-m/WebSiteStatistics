using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using DataLayer.DbContext;
using DomainClasses.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
                OnlineUsers = (int)HttpContext.Application["OnlineUsersCount"],
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
                var results = db.Statisticses.GroupBy(ua => new { ua.UserOs}).Select(g => new {lable = g.Key.UserOs, data = g.Count()}).ToArray();
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
                
                var results = db.Countries.Select(c => new {y = c.CountryName, a = c.ViewCount}).ToArray();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult RequestVisitorsVectorMapData()
        {
            var countrydata =new List<VectorMapViewModel>();
            using (var db = new AppDbContext())
            {
                
                var results = db.Countries;
                countrydata.AddRange(results.Select(country => new VectorMapViewModel
                {
                    CountryCode = country.CountryCode, CountryVisit = country.ViewCount
                }));


                return Json(countrydata, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult VectorMap()
        {
            return View();
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
                    db.BlockedIps.Add(bi);
                    db.SaveChanges();
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
                db.BlockedIps.Remove(db.BlockedIps.First(i => i.Id == Id));
                db.SaveChanges();
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
            var btv=new List<BrowserTableViewModel>();
            using (var db=new AppDbContext())
            {
                var tottal = db.Statisticses.Count();
                btv.AddRange(db.Statisticses.GroupBy(ua => new { ua.UserAgent }).OrderByDescending(g=>g.Count()).Select(g => new BrowserTableViewModel() {BrowserName = g.Key.UserAgent,BrowserViewCount = g.Count(),TottalVisits = tottal }).ToList());

            }



            return PartialView("_BrowserTablePartial",btv);
        }
        //بارگزاری اطلاعات برای جدول درصد استفاده از سیستم عامل ها
        public ActionResult OsTable()
        {
            var otv = new List<OsTableViewModel>();
            using (var db = new AppDbContext())
            {
                var tottal = db.Statisticses.Count();
                otv.AddRange(db.Statisticses.GroupBy(ua => new { ua.UserOs }).OrderByDescending(g=>g.Count()).Select(g => new OsTableViewModel() { OsName = g.Key.UserOs, OsViewCount = g.Count(), TottalVisits = tottal }).ToList());

            }



            return PartialView("_OsTablePartial", otv);
        }



    }
   
}