using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechZone.Models.ViewModels
{
    public class productCustomerDetailsViewModel
    {
        [Required]
        [DisplayName("Product Name")]
        public string  name { get; set; }
        public string? Description { get; set; }
        [Required]
        public double Price { get; set; }


        [DisplayName("Price After discount")]
        public double? DiscounPrice { get; set; }

        public string? ImageUrl { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
    }
}
