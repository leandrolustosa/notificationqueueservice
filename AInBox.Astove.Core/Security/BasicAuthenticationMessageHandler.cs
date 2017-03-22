using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AInBox.Astove.Core.Model;

namespace AInBox.Astove.Core.Security
{
    public class BasicAuthenticationMessageHandler : DelegatingHandler
    {
        public const string BasicScheme = "Basic";
        public const string ChallengeAuthenticationHeaderName = "WWW-Authenticate";
        public const char AuthorizationHeaderSeparator = ':';

        private ApplicationUserManager _userManager;

        public BasicAuthenticationMessageHandler()
        {
        }

        public BasicAuthenticationMessageHandler(ApplicationUserManager userManager)
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

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var authHeader = request.Headers.Authorization;
            if (authHeader == null)
            {
                return await CreateUnauthorizedResponse();
            }

            if (authHeader.Scheme != BasicScheme)
            {
                return await CreateUnauthorizedResponse();
            }

            var encodedCredentials = authHeader.Parameter;
            var encryptedCredentials = Encoding.UTF8.GetString(System.Web.HttpServerUtility.UrlTokenDecode(encodedCredentials));
            var credentials = RijndaelManagedClass.Decrypt(encryptedCredentials);
            var credentialParts = credentials.Split(AuthorizationHeaderSeparator);

            if (credentialParts.Length != 2)
            {
                return await CreateUnauthorizedResponse();
            }

            var username = credentialParts[0].Trim();
            var password = credentialParts[1].Trim();

            var user = await UserManager.FindAsync(username, password);
            if (user == null)
            {
                return await CreateUnauthorizedResponse();
            }

            //SetPrincipal(username);

            return await base.SendAsync(request, cancellationToken);
        }

        //private void SetPrincipal(string username)
        //{
        //    var roles = UserManager.GetRoles(username);
        //    var user = UserManager.FindById(HttpContext.Current.User.Identity.GetUserId());

        //    var identity = CreateIdentity(user.UserName);

        //    var principal = new GenericPrincipal(identity, roles.ToArray());
        //    Thread.CurrentPrincipal = principal;

        //    if (HttpContext.Current != null)
        //    {
        //        HttpContext.Current.User = principal;
        //    }
        //}

        private GenericIdentity CreateIdentity(string username)
        {
            var identity = new GenericIdentity(username, BasicScheme);
            //identity.AddClaim(new Claim(ClaimTypes.Sid, modelUser.UserId.ToString()));
            //identity.AddClaim(new Claim(ClaimTypes.GivenName, modelUser.Firstname));
            //identity.AddClaim(new Claim(ClaimTypes.Surname, modelUser.Lastname));
            //identity.AddClaim(new Claim(ClaimTypes.Email, modelUser.Email));
            return identity;
        }

        private Task<HttpResponseMessage> CreateUnauthorizedResponse()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.Headers.Add(ChallengeAuthenticationHeaderName, BasicScheme);

            var taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();
            taskCompletionSource.SetResult(response);
            return taskCompletionSource.Task;
        }
    }
}