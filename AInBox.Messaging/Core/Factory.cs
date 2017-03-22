using AInBox.Astove.Core.Data;
using Autofac;

namespace AInBox.Messaging.Core
{
    public class Factory
    {
        private static IContainer container;

        public static IEntityService<T> CreateOrGetServiceOf<T>() where T : class, IEntity, new()
        {
            if (container == null)
                container = Bootstrap.BuildContainer();

            var service = container.Resolve<IEntityService<T>>();
            return service;
        }
    }
}
