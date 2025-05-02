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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private AppDbContext _db;

        public OrderHeaderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
       

        public void Update(OrderHeader obj)
        {
            _db.orderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string Orderstatus, string? PaymentStatus = null)
        {
            var orderFrmDb=_db.orderHeaders.FirstOrDefault(x=>x.Id== id);
            if (orderFrmDb != null) 
            {
                orderFrmDb.OrderStatus = Orderstatus;
                if (!string.IsNullOrEmpty(PaymentStatus)) {  orderFrmDb.PaymentStatus = PaymentStatus;}
                
            
            }
        }

        public void UpdateStripPaymentId(int id, string sessionId, string PaymentIntentId)
        {
            var orderFrmDb = _db.orderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFrmDb != null) 
            {
                if (!string.IsNullOrEmpty(sessionId)) { orderFrmDb.SessionId = sessionId; }

                if (!string.IsNullOrEmpty(PaymentIntentId)) 
                {
                    orderFrmDb.PaymentintentId = PaymentIntentId;
                    orderFrmDb.Paymentdate=DateTime.Now;
                }
            }
        }
    }
}
