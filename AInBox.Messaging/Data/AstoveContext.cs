using AInBox.Astove.Core.Data;
using System.Data.Entity;

namespace AInBox.Messaging.Data
{
    public class AstoveContext : DbContext, IAstoveContext
    {
        public IDbSet<Enterprise> Enterprises { get; set; }
        public IDbSet<Module> Modules { get; set; }
        public IDbSet<Email> Emails { get; set; }

        public new IDbSet<T> Set<T>() where T : class, IEntity
        {
            return base.Set<T>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("");

            base.OnModelCreating(modelBuilder);
        }
    }
}
