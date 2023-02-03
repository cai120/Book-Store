using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> GetAll(string? includeProperties = null);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        T FindById(int id);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Remove(T entity);
        void RemoveRange (IEnumerable<T> entities);
        void Modify(T entity);
    }
}
