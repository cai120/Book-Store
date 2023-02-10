using BulkyBook.Models;
using BulkyBook.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void UpdateStatus(int id, OrderStatus orderStatus, PaymentStatus? paymentStatus = null);
        void UpdateStripe(int id, string sessionId, string paymentIntentId);

	}
}
