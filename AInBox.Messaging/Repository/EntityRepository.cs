using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.Filter;
using Autofac;
using System.Text.RegularExpressions;
using AInBox.Astove.Core.Logging;
using AInBox.Astove.Core.Attributes;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AInBox.Messaging.Data;

namespace AInBox.Messaging.Data
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class, IEntity, new()
    {
        readonly IAstoveContext _entitiesContext;
        readonly ILog<IEntityRepository<T>> logger;

        public EntityRepository(IAstoveContext entitiesContext, ILog<IEntityRepository<T>> logger)
        {
            if (entitiesContext == null)
            {
                throw new ArgumentNullException("entitiesContext");
            }

            _entitiesContext = entitiesContext;
            this.logger = logger;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _entitiesContext.Set<T>();
        }

        public virtual IQueryable<T> All
        {
            get { return GetAll(); }
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _entitiesContext.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public virtual IQueryable<T> AllIncluding(string[] includeProperties, bool proxyCreationEnabled = false)
        {
            IQueryable<T> query = _entitiesContext.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public virtual IQueryable<T> AllIncluding(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public virtual IQueryable<T> AllIncluding(IQueryable<T> query, string[] includeProperties)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate, bool noTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = null;
            if (includeProperties != null && includeProperties.Length > 0)
                query = AllIncluding(includeProperties);
            else
                query = All;

            query = (predicate == null) ? query : query.Where(predicate);

            if (noTracking)
                query = query.AsNoTracking();

            return query;
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate, string[] includeProperties, bool noTracking = false)
        {
            IQueryable<T> query = null;
            if (includeProperties != null && includeProperties.Length > 0)
                query = AllIncluding(includeProperties);
            else
                query = All;

            query = (predicate == null) ? query : query.Where(predicate);

            if (noTracking)
                query = query.AsNoTracking();

            return query;
        }

        public T GetSingle(int id, string[] includeProperties, bool noTracking = false)
        {
            T entity = Where(t => t.Id == id, includeProperties, noTracking).FirstOrDefault();

            return entity;
        }

        public async Task<T> GetSingleAsync(int id, string[] includeProperties, bool noTracking = false)
        {
            T entity = await Where(t => t.Id == id, includeProperties, noTracking).FirstOrDefaultAsync();

            return entity;
        }

        public T GetSingle(int id, bool noTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            T obj = Where(p => p.Id == id, noTracking, includeProperties).FirstOrDefault();
            return obj;
        }

        public async Task<T> GetSingleAsync(int id, bool noTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            T obj = await Where(p => p.Id == id, noTracking, includeProperties).FirstOrDefaultAsync();
            return obj;
        }

        public T GetSingle(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false)
        {
            T entity = Where(t => t.Id == id, noTracking).FirstOrDefault();

            return entity;
        }

        public async Task<T> GetSingleAsync(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false)
        {
            T entity = await Where(t => t.Id == id, noTracking).FirstOrDefaultAsync();

            return entity;
        }

        public T GetSingle(int id, bool noTracking = false)
        {
            return Where(t => t.Id == id, noTracking).FirstOrDefault();
        }

        public async Task<T> GetSingleAsync(int id, bool noTracking = false)
        {
            return await Where(t => t.Id == id, noTracking).FirstOrDefaultAsync();
        }

        public virtual PaginatedList<T> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector)
        {
            return Paginate(pageIndex, pageSize, keySelector, null, null);
        }

        public virtual PaginatedList<T> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = null;
            if (includeProperties != null)
                query = AllIncluding(includeProperties).OrderBy(keySelector);
            else
                query = All.OrderBy(keySelector);

            query = (predicate == null) ? query : query.Where(predicate);

            return query.ToPaginatedList(null, null, null, pageIndex, pageSize);
        }

        public async virtual Task<PaginatedList<T>> PaginateAsync<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector)
        {
            return await PaginateAsync(pageIndex, pageSize, keySelector, null, null);
        }

        public async virtual Task<PaginatedList<T>> PaginateAsync<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = null;
            if (includeProperties != null)
                query = AllIncluding(includeProperties).OrderBy(keySelector);
            else
                query = All.OrderBy(keySelector);

            query = (predicate == null) ? query : query.Where(predicate);

            return await query.ToPaginatedListAsync(null, null, null, pageIndex, pageSize);
        }

        public virtual PaginatedList<T> Paginate(IComponentContext container, AInBox.Astove.Core.Options.Filter filter, IRequestFilter cmd, Type modelType, int pageIndex, int pageSize, string[] ordersBy, string[] directions)
        {
            return Paginate(container, filter, cmd, modelType, pageIndex, pageSize, ordersBy, directions, null);
        }

        public virtual PaginatedList<T> Paginate(IComponentContext container, AInBox.Astove.Core.Options.Filter filter, IRequestFilter cmd, Type modelType, int pageIndex, int pageSize, string[] ordersBy, string[] directions, string[] includeProperties)
        {
            IQueryable<T> query = null;
            if (ordersBy != null)
                for (int i = 0; i < ordersBy.Length; i++)
                    ordersBy[i] = string.Format("{0} {1}", ordersBy[i], directions[i]);

            string orderBy = (ordersBy == null || ordersBy.Length == 0 || ordersBy.Contains("system.string[] system.string[]")) ? "Id" : string.Join(",", ordersBy);
            if (includeProperties != null)
                query = AllIncluding(includeProperties, true).OrderBy(orderBy);
            else
                query = All.OrderBy(orderBy);

            string whereClause = string.Empty;
            object[] parameters = null;
            if (cmd.Fields != null)
            {
                whereClause = FilterFactory.GetWhereClause(modelType, filter.Conditions);
                parameters = FilterFactory.GetParameters(modelType, filter.Conditions);
            }
            else if (filter.Conditions != null && filter.Conditions.Where(c => c.Internal == true).ToArray().Length > 0)
            {
                whereClause = FilterFactory.GetWhereClause(modelType, filter.Conditions.Where(c => c.Internal == true).ToArray());
                parameters = FilterFactory.GetParameters(modelType, filter.Conditions.Where(c => c.Internal == true).ToArray());
            }

            if (!string.IsNullOrEmpty(cmd.ParentValue) && cmd.ParentKey > 0)
            {
                if (string.IsNullOrEmpty(whereClause))
                    whereClause = string.Empty;

                whereClause = string.Concat(whereClause, (string.IsNullOrEmpty(whereClause)) ? string.Empty : " && ", string.Format("{0}==@{1}", cmd.ParentValue, (string.IsNullOrEmpty(whereClause)) ? 0 : Regex.Matches(whereClause, "&&").Count + 1));
                var pars = new List<object>();
                if (parameters != null)
                    pars.AddRange(parameters);
                pars.Add(cmd.ParentKey);
                parameters = pars.ToArray();
            }

            var attr = modelType.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
            if (attr != null && attr.WhereClauses != null)
            {
                if (string.IsNullOrEmpty(whereClause))
                    whereClause = string.Empty;

                whereClause = string.Concat(whereClause, (string.IsNullOrEmpty(whereClause)) ? string.Empty : " && ", string.Join(" && ", attr.WhereClauses));
            }

            query = (string.IsNullOrEmpty(whereClause)) ? query : query.Where(whereClause, parameters);

            return query.ToPaginatedList(container, filter, new KeyValue { Key = cmd.ParentKey, Value = cmd.ParentValue }, pageIndex, pageSize);
        }

        public async virtual Task<PaginatedList<T>> PaginateAsync(IComponentContext container, AInBox.Astove.Core.Options.Filter filter, IRequestFilter cmd, Type modelType, int pageIndex, int pageSize, string[] ordersBy, string[] directions)
        {
            return await PaginateAsync(container, filter, cmd, modelType, pageIndex, pageSize, ordersBy, directions, null);
        }

        public async virtual Task<PaginatedList<T>> PaginateAsync(IComponentContext container, AInBox.Astove.Core.Options.Filter filter, IRequestFilter cmd, Type modelType, int pageIndex, int pageSize, string[] ordersBy, string[] directions, string[] includeProperties)
        {
            IQueryable<T> query = null;
            if (ordersBy != null)
                for (int i = 0; i < ordersBy.Length; i++)
                    ordersBy[i] = string.Format("{0} {1}", ordersBy[i], directions[i]);

            string orderBy = (ordersBy == null || ordersBy.Length == 0 || ordersBy.Contains("system.string[] system.string[]")) ? "Id" : string.Join(",", ordersBy);
            if (includeProperties != null)
                query = AllIncluding(includeProperties, true).OrderBy(orderBy);
            else
                query = All.OrderBy(orderBy);

            string whereClause = string.Empty;
            object[] parameters = null;
            if (cmd.Fields != null)
            {
                whereClause = FilterFactory.GetWhereClause(modelType, filter.Conditions);
                parameters = FilterFactory.GetParameters(modelType, filter.Conditions);
            }
            else if (filter.Conditions != null && filter.Conditions.Where(c => c.Internal == true).ToArray().Length > 0)
            {
                whereClause = FilterFactory.GetWhereClause(modelType, filter.Conditions.Where(c => c.Internal == true).ToArray());
                parameters = FilterFactory.GetParameters(modelType, filter.Conditions.Where(c => c.Internal == true).ToArray());
            }

            if (!string.IsNullOrEmpty(cmd.ParentValue) && cmd.ParentKey > 0)
            {
                if (string.IsNullOrEmpty(whereClause))
                    whereClause = string.Empty;

                whereClause = string.Concat(whereClause, (string.IsNullOrEmpty(whereClause)) ? string.Empty : " && ", string.Format("{0}==@{1}", cmd.ParentValue, (string.IsNullOrEmpty(whereClause)) ? 0 : Regex.Matches(whereClause, "&&").Count + 1));
                var pars = new List<object>();
                if (parameters != null)
                    pars.AddRange(parameters);
                pars.Add(cmd.ParentKey);
                parameters = pars.ToArray();
            }

            var attr = modelType.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
            if (attr != null && attr.WhereClauses != null)
            {
                if (string.IsNullOrEmpty(whereClause))
                    whereClause = string.Empty;

                whereClause = string.Concat(whereClause, (string.IsNullOrEmpty(whereClause)) ? string.Empty : " && ", string.Join(" && ", attr.WhereClauses));
            }

            query = (string.IsNullOrEmpty(whereClause)) ? query : query.Where(whereClause, parameters);

            return await query.ToPaginatedListAsync(container, filter, new KeyValue { Key = cmd.ParentKey, Value = cmd.ParentValue }, pageIndex, pageSize);
        }

        public virtual List<KeyValue> GetList(string select, object[] selectParameters, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties)
        {
            if (string.IsNullOrEmpty(select))
                throw new ArgumentNullException("select");

            IQueryable<T> query = null;
            if (ordersBy != null)
                for (int i = 0; i < ordersBy.Length; i++)
                    ordersBy[i] = string.Format("{0}", ordersBy[i]);

            logger.Info("query {0}", typeof(T).Name);

            string orderBy = (ordersBy == null || ordersBy.Length == 0) ? "Id" : string.Join(",", ordersBy);
            if (includeProperties != null)
                query = AllIncluding(includeProperties, true).OrderBy(orderBy);
            else
                query = All.OrderBy(orderBy);
            logger.Info("query {0} Order by {1}", typeof(T).Name, orderBy);

            var objQuery = (string.IsNullOrEmpty(whereClause)) ? query : query.Where(whereClause, parameters);
            logger.Info("query {0} Where {1}", typeof(T).Name, whereClause);

            var lstQuery = objQuery.AsEnumerable().Select(select, selectParameters).Cast<DynamicClass>().ToList();
            logger.Info("query {0} Select {1}", typeof(T).Name, select);

            var list = new List<KeyValue>();
            logger.Info("query {0} Count {1}", typeof(T).Name, lstQuery.Count);

            foreach (dynamic obj in lstQuery)
            {
                var kv = new KeyValue();
                kv.Key = obj.Key;
                kv.Value = obj.Value;

                logger.Info("query {0} Key {1} Value {2}", typeof(T).Name, kv.Key, kv.Value);

                list.Add(kv);
            }

            return list;
        }

        public async virtual Task<List<KeyValue>> GetListAsync(string select, object[] selectParameters, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties)
        {
            if (string.IsNullOrEmpty(select))
                throw new ArgumentNullException("select");

            IQueryable<T> query = null;
            if (ordersBy != null)
                for (int i = 0; i < ordersBy.Length; i++)
                    ordersBy[i] = string.Format("{0}", ordersBy[i]);

            logger.Info("query {0}", typeof(T).Name);

            string orderBy = (ordersBy == null || ordersBy.Length == 0) ? "Id" : string.Join(",", ordersBy);
            if (includeProperties != null)
                query = AllIncluding(includeProperties, true).OrderBy(orderBy);
            else
                query = All.OrderBy(orderBy);
            logger.Info("query {0} Order by {1}", typeof(T).Name, orderBy);

            var objQuery = (string.IsNullOrEmpty(whereClause)) ? query : query.Where(whereClause, parameters);
            logger.Info("query {0} Where {1}", typeof(T).Name, whereClause);

            var lst = await objQuery.ToListAsync();
            var lstQuery = lst.AsEnumerable().Select(select, selectParameters).Cast<DynamicClass>().ToList();
            logger.Info("query {0} Select {1}", typeof(T).Name, select);

            var list = new List<KeyValue>();
            logger.Info("query {0} Count {1}", typeof(T).Name, lstQuery.Count);

            foreach (dynamic obj in lstQuery)
            {
                var kv = new KeyValue();
                kv.Key = obj.Key;
                kv.Value = obj.Value;

                logger.Info("query {0} Key {1} Value {2}", typeof(T).Name, kv.Key, kv.Value);

                list.Add(kv);
            }

            return list;
        }

        public virtual void Add(T entity)
        {
            if (_entitiesContext.GetType() != typeof(FakeAstoveContext))
            {
                DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
            }
            _entitiesContext.Set<T>().Add(entity);
        }

        public virtual void Edit(T entity)
        {
            if (_entitiesContext.GetType() != typeof(FakeAstoveContext))
            {
                DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
                dbEntityEntry.State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var dbSet = (FakeDbSet<T>)_entitiesContext.Set<T>();
                var errorMessage = dbSet.Validate(entity);
                if (!string.IsNullOrEmpty(errorMessage))
                    throw new ValidationException(errorMessage);

                var obj = dbSet.Where(o => o.Id == entity.Id).FirstOrDefault();

                obj = entity;
            }
        }

        public virtual void Delete(T entity)
        {
            if (_entitiesContext.GetType() != typeof(FakeAstoveContext))
            {
                DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
                dbEntityEntry.State = System.Data.Entity.EntityState.Deleted;
            }
            else
            {
                _entitiesContext.Set<T>().Remove(entity);
            }

        }

        public virtual void Save()
        {
            _entitiesContext.SaveChanges();
        }

        public async virtual Task SaveAsync()
        {
            await _entitiesContext.SaveChangesAsync();
        }
    }
}