using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Transactions;

namespace AInBox.Astove.Core.Security
{
    public class TokenValidation : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = string.Empty;

            IEnumerable<KeyValuePair<string, string>> queryString = request.GetQueryNameValuePairs();
            KeyValuePair<string, string> kv = queryString.FirstOrDefault(k => k.Key.Equals("token"));
            if (!string.IsNullOrEmpty(kv.Value))
                token = queryString.FirstOrDefault(k => k.Key.Equals("token")).Value;

            if (string.IsNullOrEmpty(token))
            {
                HttpResponseMessage reply = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Para acessar este serviço é necessário informar o token de acesso.");

                var taskSource = new TaskCompletionSource<HttpResponseMessage>();
                taskSource.SetResult(reply);
                return taskSource.Task;
            }

            try
            {
                
                string basicAuth = RSAClass.Decrypt(token);
                
                //TODO: implemenatr logica do login

                
                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception)
            {
                HttpResponseMessage reply = request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Usuário sem permissão acesso.");

                var taskSource = new TaskCompletionSource<HttpResponseMessage>();
                taskSource.SetResult(reply);
                return taskSource.Task;
            }            
        }
    }
}
