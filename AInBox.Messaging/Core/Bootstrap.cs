using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Logging;
using AInBox.Astove.Core.UnitTest;
using AInBox.Messaging.Data;
using AInBox.Messaging.Manager;
using Autofac;
using MongoDB.Driver;

namespace AInBox.Messaging.Core
{
    public class Bootstrap
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(System.AppDomain.CurrentDomain.GetAssemblies()).As<IAstoveUnitTest>();

            builder.RegisterGeneric(typeof(EntityService<>))
                .As(typeof(IEntityService<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<MongoClient>()
                .WithParameter("connectionString", System.Configuration.ConfigurationManager.AppSettings["MongoHost"])
                .As<IMongoClient>()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EntityRepository<>))
                .As(typeof(IEntityRepository<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<AstoveContext>()
               .As<IAstoveContext>()
               .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(NLogLogger<>)).As(typeof(ILog<>)).InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(System.AppDomain.CurrentDomain.GetAssemblies()).As<IEntity>();

            return builder.Build();
        }
    }
}