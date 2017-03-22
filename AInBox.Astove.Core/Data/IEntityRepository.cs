using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Filter;
using Autofac;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Data
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> AllIncluding(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> AllIncluding(string[] includeProperties, bool proxyCreationEnabled);
        IQueryable<T> AllIncluding(IQueryable<T> query, string[] includeProperties);

        IQueryable<T> All { get; }
        IQueryable<T> GetAll();

        T GetSingle(int id, bool noTracking = false);
        T GetSingle(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false);
        T GetSingle(int id, bool noTracking = false, params Expression<Func<T, object>>[] includeProperties);
        T GetSingle(int id, string[] includeProperties, bool noTracking = false);

        Task<T> GetSingleAsync(int id, bool noTracking = false);
        Task<T> GetSingleAsync(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false);
        Task<T> GetSingleAsync(int id, string[] includeProperties, bool noTracking = false);
        Task<T> GetSingleAsync(int id, bool noTracking = false, params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> Where(Expression<Func<T, bool>> predicate, bool noTracking = false, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate, string[] includeProperties, bool noTracking = false);

        PaginatedList<T> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector);
        PaginatedList<T> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task<PaginatedList<T>> PaginateAsync<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector);
        Task<PaginatedList<T>> PaginateAsync<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        PaginatedList<T> Paginate(IComponentContext container, AInBox.Astove.Core.Options.Filter filter, IRequestFilter cmd, Type modelType, int pageIndex, int pageSize, string[] ordersBy, string[] directions);
        PaginatedList<T> Paginate(IComponentContext container, AInBox.Astove.Core.Options.Filter filter, IRequestFilter cmd, Type modelType, int pageIndex, int pageSize, string[] ordersBy, string[] directions, string[] includeProperties);

        Task<PaginatedList<T>> PaginateAsync(IComponentContext container, AInBox.Astove.Core.Options.Filter filter, IRequestFilter cmd, Type modelType, int pageIndex, int pageSize, string[] ordersBy, string[] directions);
        Task<PaginatedList<T>> PaginateAsync(IComponentContext container, AInBox.Astove.Core.Options.Filter filter, IRequestFilter cmd, Type modelType, int pageIndex, int pageSize, string[] ordersBy, string[] directions, string[] includeProperties);

        List<KeyValue> GetList(string select, object[] selectParameters, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties);
        Task<List<KeyValue>> GetListAsync(string select, object[] selectParameters, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties);

        void Add(T entity);
        void Edit(T entity);
        void Delete(T entity);
        void Save();
        Task SaveAsync();
    }
}
