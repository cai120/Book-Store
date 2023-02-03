using BulkyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Cover = new CoverRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            Trolley = new TrolleyRepository(_db);
            User = new ApplicationUserRepository(_db);
        }
        public ICategoryRepository Category { get; private set; }
        public ICoverRepository Cover { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public ITrolleyRepository Trolley { get; private set; }
        public IApplicationUserRepository User { get; private set; }
    }
}
