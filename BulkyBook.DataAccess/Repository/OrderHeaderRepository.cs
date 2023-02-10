using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void UpdateStatus(int id, OrderStatus orderStatus, PaymentStatus? paymentStatus = null)
        {
            var order = _db.OrderHeaders.FirstOrDefault(a=>a.Id == id);
            if (order != null) 
            {
                order.OrderStatus = orderStatus;
                if(paymentStatus != null)
                {
                    order.PaymentStatus = paymentStatus.GetValueOrDefault();
                }
            }
			Save();
		}
        
        public void UpdateStripe(int id, string sessionId, string paymentIntentId)
        {
            var order = _db.OrderHeaders.FirstOrDefault(a=>a.Id == id);
            order.SessionId = sessionId;
            order.PaymentIntentId = paymentIntentId;
            Save();
        }
    }
}
