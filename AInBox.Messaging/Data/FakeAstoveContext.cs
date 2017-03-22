using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using AInBox.Astove.Core.UnitTest;
using AInBox.Astove.Core.Data;

namespace AInBox.Messaging.Data
{
    public class FakeAstoveContext : IAstoveContext
    {
        public FakeAstoveContext()
        {
            var enterprises = new List<Enterprise>
            {
                new Enterprise { Id = 1, Name = "AI'n'Box", Username = "leandrolustosa", Password = "Zwngyt="  }
            }.AsQueryable();
            this.Enterprises = new FakeDbSet<Enterprise>(new TestDbAsyncEnumerable<Enterprise>(enterprises));

            var systems = new List<Module>
            {
                new Module { Id = 1, Name = "Website", Active = true, EnterpriseId = 1, Enterprise = this.Enterprises.FirstOrDefault(o => o.Id == 1)  }
            }.AsQueryable();
            this.Systems = new FakeDbSet<Module>(new TestDbAsyncEnumerable<Module>(systems));

            var emails = new List<Email>
            {
                new Email {  }
            }.AsQueryable();
            this.Emails = new FakeDbSet<Email>(new TestDbAsyncEnumerable<Email>(emails));
        }

        public IDbSet<Enterprise> Enterprises { get; set; }
        public IDbSet<Module> Systems { get; set; }
        public IDbSet<Email> Emails { get; set; }

        public int SaveChanges()
        {
            return 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Task.Run(() => { return 0; });
        }

        public IDbSet<T> Set<T>() where T : class, IEntity
        {
            FakeDbSet<T> obj = null;
            var props = this.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType.Equals(typeof(IDbSet<T>)))
                    obj = (FakeDbSet<T>)prop.GetValue(this);
            }
            return obj;
        }

        public DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }
    }
}