using Angular_DataTable.Models;
using System.Linq;

namespace Angular_DataTable.Repository
{
    public interface ICustomerRepository
    {
        Customer GetSingle(int id);
        void Add(Customer item);
        void Delete(int id);
        Customer Update(int id, Customer item);
        IQueryable<Customer> GetAll(QueryParameters queryParameters);
        int Count();
        bool Save();
    }
}
