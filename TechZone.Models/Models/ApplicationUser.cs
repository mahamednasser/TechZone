using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechZone.Models.Models
{
    public class ApplicationUser:IdentityUser
    {


       
      

        public string? StreetAddress { get; set; }
     

        public string? City { get; set; }
     

        public string? name { get; set; }
        [NotMapped]
        public string? role { get; set; }

    }
}
