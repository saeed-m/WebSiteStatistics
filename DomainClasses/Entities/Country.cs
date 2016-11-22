using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities
{
   public class Country
    {
       public int CountryId { get; set; }
       public string CountryCode { get; set; }
       public string CountryName { get; set; }
       public string Latitude { get; set; }
       public string Longitude { get; set; }
       public int ViewCount { get; set; }



    }
}
