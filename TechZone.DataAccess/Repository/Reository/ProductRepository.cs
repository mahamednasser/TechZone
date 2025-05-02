using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechZone.DataAccess.Data;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.Models.Models;

namespace TechZone.DataAccess.Repository.Reository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        private AppDbContext _db;

        public ProductRepository(AppDbContext db):base(db) 
        {
            _db = db;
        }
        public void Update(Product obj)
        {
            _db.Products.Update(obj);
        }
    }
}
