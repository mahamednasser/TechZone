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
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }

        [ForeignKey("OrderHeaderId")]
        [ValidateNever]
        public OrderHeader OrderHeaders { get; set; }
        [Required]
        public int productId {  get; set; }
        [ForeignKey("productId")]
        [ValidateNever]
        public Product Product { get; set; }

        public int count {  get; set; }

        public double price { get; set; }
    
    }

}
