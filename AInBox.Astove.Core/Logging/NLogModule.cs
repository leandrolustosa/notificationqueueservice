using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;

namespace AInBox.Astove.Core.Logging
{
    public class NLogModule : Module
    {
        //protected override void Load(ContainerBuilder builder)
        //{
        //    builder
        //        .Register((c, p) => new NLogLogger(p.TypedAs<Type>()))
        //        .AsImplementedInterfaces();
        //}

        //protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        //{
        //    registration.Preparing +=
        //        (sender, args) =>
        //        {
        //            var forType = args.Component.Activator.LimitType;

        //            var logParameter = new ResolvedParameter(
        //                (p, c) => p.ParameterType == typeof(ILog),
        //                (p, c) => c.Resolve<ILog>(TypedParameter.From(forType)));

        //            args.Parameters = args.Parameters.Union(new[] { logParameter });
        //        };
        //}
    }
}
