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

        
        

       

    }
}
