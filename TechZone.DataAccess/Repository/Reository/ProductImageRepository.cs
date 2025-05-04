using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechZone.DataAccess.Data;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.Models.Models;

namespace TechZone.DataAccess.Repository.Reository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private AppDbContext _db;

        public ProductImageRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
       

        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
