using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechZone.Models.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey("ProductId")]
        public Product product { get; set; }
        [Range(0,1000,ErrorMessage ="The max Count is 1000")]
        public int count { get; set; }
        public string ApplicationUserId{ get; set; }

        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        [ValidateNever]
        public double? price { get; set; }

    }
}
