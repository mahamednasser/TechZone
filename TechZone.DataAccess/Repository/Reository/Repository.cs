using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
namespace TechZone.DataAccess.Repository.Reository
{
    public class Repository<T> : IRepository<T> where T : class
    { 
        private readonly AppDbContext _db;
        internal DbSet<T> dbset;
        public Repository( AppDbContext db)
        {
            _db = db;
            this.dbset = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbset.Add(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? IncludeProp = null)
        {
            IQueryable<T> AllEntities = dbset;
            if (filter != null)
            {
               AllEntities=AllEntities.Where(filter );

            }
            if (!string.IsNullOrEmpty(IncludeProp))
            {
                foreach (var prop in IncludeProp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    AllEntities = AllEntities.Include(prop);
                }
            }
                return AllEntities;
        }

        public T GetById(Expression<Func<T, bool>> Filter, string? IncludeProp = null,bool tracked=false)
        {
            IQueryable<T> element;
            if (tracked)
            {
               element = dbset;
            }
            else
            {
                element = dbset.AsNoTracking();
            }
                
            element=element.Where(Filter);
            if (!string.IsNullOrEmpty(IncludeProp))
            {
                foreach (var prop in IncludeProp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    element = element.Include(prop);
                }
            }
            return element.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbset.RemoveRange(entities);
        }
    }
}
