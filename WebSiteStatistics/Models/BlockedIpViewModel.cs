using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace WebSiteStatistics.Models
{
    public class BlockedIpViewModel
    {
        
        public int Id { get; set; }
        [Required(ErrorMessage = "شماره آی پی را وارد نکرده اید")]
        [Display(Name = "شماره آی پی")]
        public string IpAddress { get; set; }
    }
}