using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IRepository : IReadOnlyRepository
    {
        void Create<TEntity>(TEntity entity, string createdBy = null)
           where TEntity : class;

        void CreateList<TEntity>(List<TEntity> entities, string createdBy = null)
          where TEntity : class;

        void Update<TEntity>(TEntity entity, object modified = null)
            where TEntity : class;

        void Delete<TEntity>(object id)
            where TEntity : class;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class;

        void Save();

        Task SaveAsync();

        void SqlQuery<TEntity>(string sqlQuery) where TEntity : class;

        void SqlQuery(string sqlQuery);

        void Reload<TEntity>(object id) where TEntity : class;

        IList<TEntity> GetDataSqlQuery<TEntity>(string query) where TEntity : class;

        public IDbContextTransaction CreateTransAction();

        public void RollbackTransAction();

        public void CommitTransAction();
    }
}