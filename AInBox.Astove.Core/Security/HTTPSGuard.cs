using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace AInBox.Astove.Core.Security
{
    public class HTTPSGuard : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.RequestUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                HttpResponseMessage reply = request.CreateErrorResponse(HttpStatusCode.BadRequest, "HTTPS is required for security reason.");
                
                var taskSource = new TaskCompletionSource<HttpResponseMessage>();
                taskSource.SetResult(reply);
                return taskSource.Task;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
