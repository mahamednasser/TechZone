using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechZone.Models.Models;

namespace TechZone.Models.ViewModels
{
    public class HomeCategoryBrandProduct
    {
        public IEnumerable<Brand>? Brands { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Product>? Products { get; set; }

    }
}
