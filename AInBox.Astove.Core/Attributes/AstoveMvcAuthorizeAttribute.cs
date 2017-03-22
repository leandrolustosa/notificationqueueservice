using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading;
using System.Security.Principal;
using System.Web.Security;
using AInBox.Astove.Core.Security;
using AInBox.Astove.Core.Model;
using System.Web;
using System.Web.Http.Owin;
using Microsoft.Owin.Security;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AstoveMvcAuthorizeAttribute : AuthorizeAttribute
    {
        private ApplicationUserManager _userManager;

        public AstoveMvcAuthorizeAttribute()
        {
        }

        public AstoveMvcAuthorizeAttribute(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //if(WebSecurity.IsAuthenticated)
            if (AuthenticationManager.User.Identity.IsAuthenticated)
            {
                string[] rolesRequired = Roles.Split(new[] { "," }, StringSplitOptions.None);
                RolePrincipal principal = (RolePrincipal)System.Web.HttpContext.Current.User;
                if (rolesRequired.Length > 0 && rolesRequired.Where(r => principal.IsInRole(r)).Count() == 0)
                {
                    filterContext.Result = new RedirectResult("~/Account/Unauthorized");
                }
            }
            else if (AuthenticationManager.User.Identity.IsAuthenticated)
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
