using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;
using DataLayer.DbContext;
using DomainClasses.Entities;
using UAParser;

namespace WebSiteStatistics.Modules
{
    public class StatModule : IHttpModule
    {
        public StatModule()
        {
            
        }
        public void Dispose()
        {
           
        }

        public void Init(HttpApplication context)
        {
            IHttpModule module = context.Modules["Session"];
            if (module.GetType() == typeof(SessionStateModule))
            {
                SessionStateModule stateModule = (SessionStateModule)module;
                stateModule.Start += (Session_Start);
                
            }
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            IList<BlockedIp> bi;
            using (var db = new AppDbContext())
            {
                bi = db.BlockedIps.AsNoTracking().ToList();

            }
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            //بررسی برای اینکه درخواست کننده موتور جستجوگر است ؟
            //و یا اینکه در لیست ای پی هایی است که نباید در آمار آورده شوند
            if (!context.Request.Browser.Crawler || !bi.Any(ip => ip.IpAddress.Equals(GetIPAddress())))
            {

                var statistic = new Statistics();
                statistic.IpAddress = GetIPAddress();
                statistic.UserOs = GetUserOS(context.Request.UserAgent);
                statistic.PageViewed = context.Request.Url.AbsolutePath;
                statistic.Referer = context.Request.UrlReferrer?.AbsoluteUri ?? "Direct";
                statistic.UserAgent = context.Request.Browser.Browser;
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