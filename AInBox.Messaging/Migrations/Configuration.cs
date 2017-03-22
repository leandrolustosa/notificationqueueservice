namespace AInBox.Messaging.Migrations
{
    using Astove.Core.Extensions;
    using Data;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AInBox.Messaging.Data.AstoveContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AInBox.Messaging.Data.AstoveContext context)
        {
            context.Enterprises.AddOrUpdate(
              p => p.Username,
              new Enterprise { Name = "AI'n'Box", Username = "ainbox", Password = "Tfghs$yhjuHGZl".Encrypt(), FromName = "Leandro Lustosa", FromEmail = "leandro@ainbox.com.br", Host = "smtplw.com.br", Port = 587, ServerUsername = "leandrolustosa", ServerPassword = "QMBYYQVt6986".Encrypt() },
              new Enterprise { Name = "Fricote", Username = "fricote", Password = "Tfghs$yhjuHGZl".Encrypt(), FromName = "Jamile", FromEmail = "fricote@lojafricote.com.br", Host = "smtplw.com.br", Port = 587, ServerUsername = "leandrolustosa", ServerPassword = "QMBYYQVt6986".Encrypt() },
              new Enterprise { Name = "Digigrafos", Username = "digigrafos", Password = "Tfghs$yhjuHGZl".Encrypt(), FromName = "Contato", FromEmail = "contato@unicacomovoce.com.br", Host = "smtplw.com.br", Port = 587, ServerUsername = "leandrolustosa", ServerPassword = "QMBYYQVt6986".Encrypt() },
              new Enterprise { Name = "Buscavita", Username = "buscavita", Password = "Tfghs$yhjuHGZl".Encrypt(), FromName = "Contato", FromEmail = "contato@buscavita.com.br", Host = "smtplw.com.br", Port = 587, ServerUsername = "leandrolustosa", ServerPassword = "QMBYYQVt6986".Encrypt() }
            );

            context.SaveChanges();

            context.Modules.AddOrUpdate(
              p => new { p.EnterpriseId, p.Name },
              new Module { Active = true, EnterpriseId = context.Enterprises.FirstOrDefault(o => o.Username.Equals("ainbox")).Id, Name = "Website", RestrictIps = "50.62.168.151" },
              new Module { Active = true, EnterpriseId = context.Enterprises.FirstOrDefault(o => o.Username.Equals("fricote")).Id, Name = "Sistema de Pedidos", RestrictIps = "23.96.109.82" },
              new Module { Active = true, EnterpriseId = context.Enterprises.FirstOrDefault(o => o.Username.Equals("digigrafos")).Id, Name = "Única como Você", RestrictIps = "23.96.109.82" },
              new Module { Active = true, EnterpriseId = context.Enterprises.FirstOrDefault(o => o.Username.Equals("digigrafos")).Id, Name = "Digijobs", FromName = "Site", FromEmail = "site@digigrafos.com.br", RestrictIps = "23.96.109.82" },
              new Module { Active = true, EnterpriseId = context.Enterprises.FirstOrDefault(o => o.Username.Equals("buscavita")).Id, Name = "Sistema de Busca de Preços", RestrictIps = "23.96.109.82" }
            );
        }
    }
}
