using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechZone.Models.Models;

namespace TechZone.Models.ViewModels
{
    public class shoppingCartViewModel
    {
      
        public IEnumerable<ShoppingCart>ShoppingCarts { get; set; }
        public OrderHeader orderHeader { get; set; }
        
       


    }
}
