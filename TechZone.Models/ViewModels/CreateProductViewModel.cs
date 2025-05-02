using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechZone.Models.Models;

namespace TechZone.Models.ViewModels
{
    public class CreateProductViewModel
    {
        
        public int? Id { get; set; }
        [Required(ErrorMessage = "The Product Name is Required")]
        [MaxLength(100, ErrorMessage = "Max Length Is 100 character")]
        public string name { get; set; }
        public string? Description { get; set; }
        [Required]
        [DisplayName("Original Price")]
        public double Price { get; set; }


        [DisplayName("Price After discount")]
        public double? DiscounPrice { get; set; }

        public string? ImageUrl { get; set; }
        [Required(ErrorMessage ="The Category Is Required")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        public List<Category>? CategoryList { get; set; }
    }
}
