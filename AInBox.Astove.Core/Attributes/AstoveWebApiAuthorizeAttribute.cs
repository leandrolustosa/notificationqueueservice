using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Security;
using System.Security.Principal;
using System.Web;
using System.Threading;
using AInBox.Astove.Core.Security;
using AInBox.Astove.Core.Model;
using System.Web.Http.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AInBox.Astove.Core.Attributes
{
    public class AstoveWebApiAuthorizeAttribute : AuthorizeAttribute
    {
        public const string BasicScheme = "Basic";
        public const string ChallengeAuthenticationHeaderName = "WWW-Authenticate";
        public const char AuthorizationHeaderSeparator = ':';

        private ApplicationUserManager _userManager;

        public AstoveWebApiAuthorizeAttribute()
        {
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

        public virtual bool BypassValidation { get; set; }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (BypassValidation)
                return true;
            
            if (AuthenticationManager.User.Identity.IsAuthenticated)
            {
                string[] rolesRequired = Roles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                RolePrincipal principal = (RolePrincipal)System.Web.HttpContext.Current.User;
                if (rolesRequired.Length > 0 && rolesRequired.Where(r => principal.IsInRole(r)).Count() == 0)
                {
                    return false;
                }

                return true;
            }
            else if (actionContext.Request.Headers.Authorization != null)
            {
                var authHeader = actionContext.Request.Headers.Authorization;
                
                if (authHeader.Scheme != BasicScheme)
                    return false;
                
                var encodedCredentials = authHeader.Parameter;
                var encryptedCredentials = RijndaelManagedClass.UrlTokenDecode(encodedCredentials);
                var credentials = RijndaelManagedClass.Decrypt(encryptedCredentials);
                var credentialParts = credentials.Split(AuthorizationHeaderSeparator);

                if (credentialParts.Length != 2)
                    return false;
                
                var username = credentialParts[0].Trim();
                var password = credentialParts[1].Trim();

                var user = UserManager.Find(username, password);
                if (user != null)
                {
                    SetPrincipal(user);

                    string[] rolesRequired = Roles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    GenericPrincipal principal = (GenericPrincipal)System.Web.HttpContext.Current.User;

                    if (rolesRequired.Length > 0 && rolesRequired.Where(r => principal.IsInRole(r.Trim())).Count() == 0)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    AuthenticationManager.SignOut();
                    return false;
                }

            }
            else if (!AuthenticationManager.User.Identity.IsAuthenticated)
            {
                return false;
            }

            return base.IsAuthorized(actionContext);
        }

        //private void SetPrincipal(string username)
        //{            
        //    var user = UserManager.FindById(AuthenticationManager.User.Identity.GetUserId());
        //    var roles = UserManager.GetRoles(AuthenticationManager.User.Identity.GetUserId());

        //    var identity = CreateIdentity(user.UserName);

        //    var principal = new GenericPrincipal(identity, roles.ToArray());
        //    Thread.CurrentPrincipal = principal;

        //    if (HttpContext.Current != null)
        //    {
        //        HttpContext.Current.User = principal;
        //    }
        //}

        private void SetPrincipal(ApplicationUser user)
        {
            var roles = UserManager.GetRoles(user.Id);

            var identity = CreateIdentity(user.UserName);

            var principal = new GenericPrincipal(identity, (roles!=null) ? roles.ToArray() : null);
            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private GenericIdentity CreateIdentity(string username)
        {
            var identity = new GenericIdentity(username, BasicScheme);
            return identity;
        }
    }
}
