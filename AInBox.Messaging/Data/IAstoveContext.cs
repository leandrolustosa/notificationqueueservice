using AInBox.Astove.Core.Data;
using AInBox.Messaging.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Messaging.Data
{
    public interface IAstoveContext
    {
        IDbSet<T> Set<T>() where T : class, IEntity;
        DbEntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}