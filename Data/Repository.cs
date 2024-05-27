using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Repository<TContext> : ReadOnlyRepository<TContext>, IRepository where TContext : DbContext
    {
        public Repository(TContext context) : base(context)
        {
        }

        IDbContextTransaction IRepository.CreateTransAction()
        {
            return base._context.Database.BeginTransaction();
        }

        public void RollbackTransAction()
        {
            base._context.Database.RollbackTransaction();
        }

        public void CommitTransAction()
        {
            base._context.Database.CommitTransaction();
        }

        public void Create<TEntity>(TEntity entity, string createdBy = null) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void CreateList<TEntity>(List<TEntity> entities, string createdBy = null) where TEntity : class
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public virtual void Delete<TEntity>(object id)
            where TEntity : class
        {
            TEntity entity = _context.Set<TEntity>().Find(id);
            Delete(entity);
        }

        public virtual void Delete<TEntity>(TEntity entity)
            where TEntity : class
        {
            var dbSet = _context.Set<TEntity>();
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public IList<TEntity> GetDataSqlQuery<TEntity>(string query) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Reload<TEntity>(object id) where TEntity : class
        {
            var entity = _context.Set<TEntity>().Find(id);
            _context.Entry(entity).Reload();
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                ThrowEnhancedValidationException(e);
            }
        }

        public Task SaveAsync()
        {
            try
            {
                return _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                ThrowEnhancedValidationException(e);
            }

            return Task.FromResult(0);
        }

        public void SqlQuery<TEntity>(string sqlQuery) where TEntity : class
        {
            //_context.Database.ExecuteSqlCommand(sqlQuery);
        }

        public void SqlQuery(string sqlQuery)
        {
            //_context.Database.ExecuteSqlCommand(sqlQuery);
        }

        public void Update<TEntity>(TEntity entity, object modified = null) where TEntity : class
        {
            _context.Set<TEntity>().Attach(entity);

            if (null == modified)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var entry = _context.Entry(entity);

                foreach (PropertyInfo property in modified.GetType().GetProperties())
                {
                    entry.Property(property.Name).IsModified = true;
                }
            }
        }

        protected virtual void ThrowEnhancedValidationException(DbEntityValidationException e)
        {
            var errorMessages = e.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

            var fullErrorMessage = string.Join("; ", errorMessages);
            var exceptionMessage = string.Concat(e.Message, " The validation errors are: ", fullErrorMessage);
            throw new DbEntityValidationException(exceptionMessage, e.EntityValidationErrors);
        }
    }
}