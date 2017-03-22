using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using Autofac;
using AInBox.Astove.Core.Filter;
using AInBox.Astove.Core.Logging;
using AInBox.Astove.Core.Extensions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AInBox.Messaging.Manager
{
    public class EntityService<TEntity> : IEntityService<TEntity>
        where TEntity : class, IEntity, new()
    {
        private readonly IEntityRepository<TEntity> context;
        private readonly IMongoClient mongoClient;
        private readonly IMongoDatabase mongoDatabase;
        private readonly ILog<IEntityService<TEntity>> logger;

        public IEntityRepository<TEntity> Repository { get { return context; } }
        public IMongoDatabase MongoDatabase { get { return mongoDatabase; } }

        public EntityService(IEntityRepository<TEntity> context, IMongoClient mongoClient, ILog<IEntityService<TEntity>> logger)
        {
            this.context = context;
            this.mongoClient = mongoClient;
            this.mongoDatabase = mongoClient.GetDatabase(System.Configuration.ConfigurationManager.AppSettings["MongoDatabase"]);
            this.logger = logger;
        }

        public HashSet<TEntity> GetEntities()
        {
            return context.GetAll().ToHashSet();
        }

        public PaginatedList<TEntity> GetEntities(IComponentContext container, IRequestFilter cmd, Type modelType, string[] ordersBy, string[] directions, string[] includeProperties)
        {
            Filter filter = FilterFactory.GenerateFilter(container, cmd, modelType, new KeyValue { Key = cmd.ParentKey, Value = cmd.ParentValue });
            return context.Paginate(container, filter, cmd, modelType, cmd.Page, cmd.Take, ordersBy, directions, includeProperties);
        }

        public async Task<PaginatedList<TEntity>> GetEntitiesAsync(IComponentContext container, IRequestFilter cmd, Type modelType, string[] ordersBy, string[] directions, string[] includeProperties)
        {
            Filter filter = FilterFactory.GenerateFilter(container, cmd, modelType, new KeyValue { Key = cmd.ParentKey, Value = cmd.ParentValue });
            return await context.PaginateAsync(container, filter, cmd, modelType, cmd.Page, cmd.Take, ordersBy, directions, includeProperties);
        }

        public List<KeyValue> GetEntities(string select, object[] selectParametrs, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties)
        {
            logger.Info("Select {0} from {1} where {2} order by {3} including {4}", new[] { select, typeof(TEntity).Name, whereClause, (ordersBy == null) ? "" : string.Join(", ", ordersBy), (includeProperties == null) ? "" : string.Join(", ", includeProperties) });
            return context.GetList(select, selectParametrs, whereClause, parameters, ordersBy, includeProperties);
        }

        public async Task<List<KeyValue>> GetEntitiesAsync(string select, object[] selectParametrs, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties)
        {
            logger.Info("Select {0} from {1} where {2} order by {3} including {4}", new[] { select, typeof(TEntity).Name, whereClause, (ordersBy == null) ? "" : string.Join(", ", ordersBy), (includeProperties == null) ? "" : string.Join(", ", includeProperties) });
            return await context.GetListAsync(select, selectParametrs, whereClause, parameters, ordersBy, includeProperties);
        }

        public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return context.Where(predicate, noTracking, includeProperties);
        }

        public TEntity GetSingle(int id, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            TEntity obj = context.GetSingle(id, noTracking, includeProperties);
            return obj;
        }

        public async Task<TEntity> GetSingleAsync(int id, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            TEntity obj = await context.GetSingleAsync(id, noTracking, includeProperties);
            return obj;
        }

        public TEntity GetSingle(int id, string[] includeProperties, bool noTracking = false)
        {
            TEntity obj = context.GetSingle(id, includeProperties, noTracking);
            return obj;
        }

        public async Task<TEntity> GetSingleAsync(int id, string[] includeProperties, bool noTracking = false)
        {
            TEntity obj = await context.GetSingleAsync(id, includeProperties, noTracking);
            return obj;
        }

        public virtual TEntity GetSingle(int id, bool noTracking = false)
        {
            TEntity obj = context.GetSingle(id, noTracking);
            return obj;
        }

        public async virtual Task<TEntity> GetSingleAsync(int id, bool noTracking = false)
        {
            TEntity obj = await context.GetSingleAsync(id, noTracking);
            return obj;
        }

        public virtual TEntity GetSingle(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false)
        {
            TEntity obj = context.GetSingle(container, cmd, id, noTracking);
            return obj;
        }

        public async virtual Task<TEntity> GetSingleAsync(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false)
        {
            TEntity obj = await context.GetSingleAsync(container, cmd, id, noTracking);
            return obj;
        }

        public virtual int Add(TEntity entity)
        {
            context.Add(entity);
            context.Save();
            return entity.Id;
        }

        public async virtual Task<int> AddAsync(TEntity entity)
        {
            context.Add(entity);
            await context.SaveAsync();
            return entity.Id;
        }

        public virtual void Edit(int id, TEntity entity)
        {
            context.Edit(entity);
            context.Save();
        }

        public async virtual Task EditAsync(int id, TEntity entity)
        {
            context.Edit(entity);
            await context.SaveAsync();
        }

        public virtual void Delete(int id)
        {
            TEntity obj = context.GetSingle(id);
            context.Delete(obj);
            context.Save();
        }

        public async virtual Task DeleteAsync(int id)
        {
            TEntity obj = await context.GetSingleAsync(id);
            context.Delete(obj);
            await context.SaveAsync();
        }
    }
}