//using System;
//using System.Data.Entity;
//using System.Linq;
//using Unco.SimpleMembershipProvider;
////using WebMatrix.WebData;

//namespace AInBox.Astove.Core.Security
//{
//    public class SimpleMembershipInitializer
//    {
//        public SimpleMembershipInitializer()
//        {
//            //Database.SetInitializer<MySqlSecurityDbContext>(null);

//            try
//            {
//                //MySqlWebSecurity.InitializeDatabaseConnection("ConnectionString");
//                WebSecurity.InitializeDatabaseConnection("MySqlDefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

//                const string ADMIN_ROLES = "Administrador";
//                const string ADMIN_USER = "suporte@AInBox.com.br";

//                if (System.Web.Security.Roles.RoleExists(ADMIN_ROLES) == false)
//                    System.Web.Security.Roles.CreateRole(ADMIN_ROLES);

//                if (WebSecurity.UserExists(ADMIN_USER) == false)
//                    WebSecurity.CreateUserAndAccount(ADMIN_USER, "senha");

//                if (System.Web.Security.Roles.GetRolesForUser(ADMIN_USER).Contains(ADMIN_ROLES) == false)
//                    System.Web.Security.Roles.AddUsersToRoles(new[] { ADMIN_USER }, new[] { ADMIN_ROLES });
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
//            }
//        }
//    }
//}