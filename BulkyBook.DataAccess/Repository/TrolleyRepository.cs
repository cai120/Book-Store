using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class TrolleyRepository : Repository<Trolley>, ITrolleyRepository
    {
        private ApplicationDbContext _db;
        public TrolleyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void IncrementCount(Trolley trolley, int amountBy)
		{
            trolley.Count += amountBy;
            _db.SaveChanges();
		}
	}
}
