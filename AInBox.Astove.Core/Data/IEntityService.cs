using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Options;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using AInBox.Astove.Core.Filter;
using System.Threading.Tasks;
using AInBox.Astove.Core.Data;
using MongoDB.Driver;

namespace AInBox.Astove.Core.Data
{
    public interface IEntityService<TEntity> 
        where TEntity : class, IEntity, new()
    {
        IEntityRepository<TEntity> Repository { get; }
        IMongoDatabase MongoDatabase { get; }

        HashSet<TEntity> GetEntities();

        PaginatedList<TEntity> GetEntities(IComponentContext container, IRequestFilter cmd, Type modelType, string[] ordersBy, string[] directions, string[] includeProperties);
        Task<PaginatedList<TEntity>> GetEntitiesAsync(IComponentContext container, IRequestFilter cmd, Type modelType, string[] ordersBy, string[] directions, string[] includeProperties);

        List<KeyValue> GetEntities(string select, object[] selectParameters, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties);
        Task<List<KeyValue>> GetEntitiesAsync(string select, object[] selectParameters, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties);

        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity GetSingle(int id, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetSingleAsync(int id, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity GetSingle(int id, string[] includeProperties, bool noTracking = false);
        Task<TEntity> GetSingleAsync(int id, string[] includeProperties, bool noTracking = false);
        TEntity GetSingle(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false);
        Task<TEntity> GetSingleAsync(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false);
        TEntity GetSingle(int id, bool noTracking = false);
        Task<TEntity> GetSingleAsync(int id, bool noTracking = false);

        int Add(TEntity entity);
        Task<int> AddAsync(TEntity entity);

        void Edit(int id, TEntity entity);
        Task EditAsync(int id, TEntity entity);

        void Delete(int id);
        Task DeleteAsync(int id);
    }
}
