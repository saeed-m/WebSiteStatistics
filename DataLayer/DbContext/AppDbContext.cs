using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainClasses.Entities;

namespace DataLayer.DbContext
{
   public class AppDbContext:System.Data.Entity.DbContext
    {
       public DbSet<Country> Countries { get; set; }
       public DbSet<BlockedIp> BlockedIps { get; set; }
       public DbSet<Statistics> Statisticses { get; set; }

       public AppDbContext():base("AppConnectionString")
       {

           
       }


    }
}
