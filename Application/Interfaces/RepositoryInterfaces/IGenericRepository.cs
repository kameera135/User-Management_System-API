using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.Interfaces.RepositoryInterfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        T GetById(int id);

        bool Add(T entity);

        bool Update(T entity);

        bool Delete(T entity);

    }
}
