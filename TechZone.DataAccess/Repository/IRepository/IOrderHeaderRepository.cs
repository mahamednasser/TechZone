using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechZone.Models.Models;
using TechZone.DataAccess.Data;

namespace TechZone.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
        void UpdateStatus(int id , string Orderstatus,string? PaymentStatus = null );
        void UpdateStripPaymentId(int id, string sessionId, string PaymentIntentId );


    }
}
