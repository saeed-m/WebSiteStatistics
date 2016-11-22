using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer.DbContext;
using DomainClasses.Entities;
using Newtonsoft.Json;
using WebSiteStatistics.Models;

namespace WebSiteStatistics.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {


            return View();
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
            using (var db = new AppDbContext())
            {

                var countries = db.Countries.AsNoTracking().ToList();

                return View(countries);
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
        public ActionResult BlockeIps()
        {
            IList<BlockedIpViewModel> bivm=new List<BlockedIpViewModel>();
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
            var ips=new List<BlockedIpViewModel>();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                using (var db = new AppDbContext())
                {
                    var bi = new BlockedIp {IpAddress = ipAddress};
                    db.BlockedIps.Add(bi);
                    db.SaveChanges();
                    var ipss = db.BlockedIps.AsNoTracking().ToList();
                    ips.AddRange(ipss.Select(ipaddrss => new BlockedIpViewModel()
                    {
                        Id = ipaddrss.Id, IpAddress = ipaddrss.IpAddress
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
    }
}