using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using DataLayer.DbContext;
using DomainClasses.Entities;
using UAParser;

namespace WebSiteStatistics
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, DataLayer.Migrations.Configuration>());
            using (var context = new AppDbContext())
            {
                context.Database.Initialize(force: true);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            IList<BlockedIp> bi;
            using (var db=new AppDbContext())
            {
                bi =db.BlockedIps.AsNoTracking().ToList();

            }
            //بررسی برای اینکه درخواست کننده موتور جستجوگر است ؟
            //و یا اینکه در لیست ای پی هایی است که نباید در آمار آورده شوند
            Application.Lock();
            if (!Request.Browser.Crawler || !bi.Any(ip => ip.IpAddress.Equals(GetIPAddress())))
            {
                #region Online UserCount

                if (Application["OnlineUsersCount"] == null)
                {
                    Application["OnlineUsersCount"] = 1;

                }
                else
                {
                    var cnt = (int) Application["OnlineUsersCount"];
                    Application["OnlineUsersCount"] = ++cnt;
                }

                #endregion

                var statistic = new Statistics();
                statistic.IpAddress = GetIPAddress();
                statistic.UserOs = GetUserOS(Request.UserAgent);
                statistic.PageViewed = HttpContext.Current.Request.Url.AbsolutePath;
                statistic.Referer = Request.UrlReferrer?.ToString() ?? "Direct";
                statistic.UserAgent = Request.Browser.Browser;
                statistic.DateStamp = DateTime.Now;
                using (var db = new AppDbContext())
                {
                    db.Statisticses.Add(statistic);
                    db.SaveChanges();
                }
                //بدست آوردن کشور بازدید کننده
                XDocument xdoc = XDocument.Load("http://www.freegeoip.net/xml/" + GetIPAddress());
                var country = xdoc.Descendants("Response").Select(c => new
                    {
                        IpAddress = c.Element("IP")?.Value,
                        CountryCode = c.Element("CountryCode")?.Value,
                        CountryName = c.Element("CountryName")?.Value,
                        RegionCode = c.Element("RegionCode")?.Value,
                        RegionName = c.Element("RegionName")?.Value,
                        City = c.Element("City")?.Value,
                        ZipCode = c.Element("ZipCode")?.Value,
                        TimeZone = c.Element("TimeZone")?.Value,
                        Latitude = c.Element("Latitude")?.Value,
                        Longitude = c.Element("Longitude")?.Value,
                        MetroCode = c.Element("MetroCode")?.Value,
                    });
                    var countryData = country.First();
                    //Check If The Country Is already in database or not
                    using (var db = new AppDbContext())
                    {
                        if (db.Countries.Any(c => c.CountryCode.Equals(countryData.CountryCode)))
                        {
                            //then Update the ViewCount
                            Country currentCountry =
                                db.Countries.First(cc => cc.CountryCode.Equals(countryData.CountryCode));
                            currentCountry.ViewCount++;
                            db.SaveChanges();
                        }
                        else
                        {
                            //then add this Country To Database
                            var newCountry = new Country()
                            {
                                CountryCode = countryData.CountryCode,
                                CountryName = countryData.CountryName,
                                Latitude = countryData.Latitude,
                                Longitude = countryData.Longitude,
                                ViewCount = 1
                            };
                            db.Countries.Add(newCountry);
                            db.SaveChanges();

                        }
                    }

                
            }
            Application.UnLock();



        }
        void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            if (Application["OnlineUsersCount"] != null)
            {
                var cnt = (int)Application["OnlineUsersCount"];
                Application["OnlineUsersCount"] = --cnt;

            }
            Application.UnLock();
        }

        #region
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
#endregion

    }
}
