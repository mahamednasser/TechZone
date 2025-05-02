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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private AppDbContext _db;

        public OrderDetailRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
       

        public void Update(OrderDetail obj)
        {
            _db.orderDetails.Update(obj);
        }
    }
}
