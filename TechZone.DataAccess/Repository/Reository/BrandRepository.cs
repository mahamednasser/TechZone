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
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private AppDbContext _db;

        public BrandRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
       

        public void Update(Brand obj)
        {
            _db.brands.Update(obj);
        }
    }
}
