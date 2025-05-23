﻿using System;
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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private AppDbContext _db;

        public CategoryRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
       

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
