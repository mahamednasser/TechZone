using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechZone.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "The Product Name is Required")]
        [MaxLength(100, ErrorMessage = "Max Length Is 100 character")]
        public string name { get; set; }

        public string? Description { get; set; }
        [Required]
        [DisplayName("OriginalPrice")]
        public double Price { get; set; }

       
        [DisplayName("Price After discount")]
        public double? DiscounPrice  { get; set; }
        [DisplayName("Products Count in stock")]
        public int? ProductCount { get; set; }
        public int? BrandId { get; set; }
        [ForeignKey("BrandId")]
        [ValidateNever]
       public Brand Brand { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
    }
}
