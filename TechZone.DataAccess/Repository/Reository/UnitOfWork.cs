using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechZone.DataAccess.Data;
using TechZone.DataAccess.Repository.IRepository;

namespace TechZone.DataAccess.Repository.Reository
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;
        public ICategoryRepository Category {  get;private set; }
        public IShoppingCartRepository ShoppingCart { get;private set; }

        public IProductRepository Product {  get;private set; }
        public IBrandRepository Brand { get;private set; }
        public IApplicationUserRepository ApplicationUser { get;private set; }
        public IOrderDetailRepository OrderDetail { get;private set; }
        public IOrderHeaderRepository OrderHeader { get;private set; }
        public IProductImageRepository ProductImage { get;private set; }

        public UnitOfWork( AppDbContext db)
        {
            _db = db;
            Category =new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Brand=new BrandRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            ProductImage = new ProductImageRepository(_db);
        }

        public void save()
        {
            _db.SaveChanges();
            
        }

       
    }
}
