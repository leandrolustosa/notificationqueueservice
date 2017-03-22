using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Model;
using Autofac;

namespace AInBox.Astove.Core.Data
{
    public interface IEntity
    {
        int Id { get; set; }

        TModel ToModel<TModel, TInterfaceConstraint>(IComponentContext container, bool lazyLoading, int level) where TModel : TInterfaceConstraint, new();
    }
}
