using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechZone.Models.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Name { get;set; }
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public string? ResourceName { get; set; }
        public ICollection<Product>? Products { get; set; }

    }
}
