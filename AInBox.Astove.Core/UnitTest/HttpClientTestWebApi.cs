using AInBox.Astove.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.UnitTest
{
    public class HttpClientTestWebApi
    {
        public const string BEARER = "Bearer ";

        public static async Task<HttpResponseMessage> PostWebApi<T>(string actionName, T model, string bearerToken = null) where T : class, IBindingModel
        {
            if (actionName == null)
                    throw new ArgumentNullException("actionName");

            if (!string.IsNullOrEmpty(bearerToken) && bearerToken.StartsWith(BEARER))
                bearerToken = string.Concat(BEARER, bearerToken);

            var httpClient = HttpClientFactory.Create();
            if (!string.IsNullOrEmpty(bearerToken))
                httpClient.DefaultRequestHeaders.Add("Authorization", bearerToken);

            var baseUri = System.Configuration.ConfigurationManager.AppSettings["WebApiUrl"];
            httpClient.BaseAddress = new Uri(baseUri);

            var response = await httpClient.PostAsync<T>(string.Concat("/api/ac/", actionName), model, new JsonMediaTypeFormatter());

            return response;         
        }

        public static async Task<HttpResponseMessage> PutWebApi<T>(string actionName, T model, string bearerToken = null) where T : class, IModel
        {
            if (actionName == null)
                throw new ArgumentNullException("actionName");

            if (!string.IsNullOrEmpty(bearerToken) && bearerToken.StartsWith(BEARER))
                bearerToken = string.Concat(BEARER, bearerToken);

            var httpClient = HttpClientFactory.Create();
            if (!string.IsNullOrEmpty(bearerToken))
                httpClient.DefaultRequestHeaders.Add("Authorization", bearerToken);

            var baseUri = System.Configuration.ConfigurationManager.AppSettings["WebApiUrl"];
            httpClient.BaseAddress = new Uri(baseUri);

            var response = await httpClient.PutAsync<T>(string.Concat("/api/ac/", actionName), model, new JsonMediaTypeFormatter());

            return response;
        }

        public static async Task<HttpResponseMessage> DeleteWebApi(string actionName, string bearerToken = null)
        {
            if (actionName == null)
                throw new ArgumentNullException("actionName");

            if (!string.IsNullOrEmpty(bearerToken) && bearerToken.StartsWith(BEARER))
                bearerToken = string.Concat(BEARER, bearerToken);

            var httpClient = HttpClientFactory.Create();
            if (!string.IsNullOrEmpty(bearerToken))
                httpClient.DefaultRequestHeaders.Add("Authorization", bearerToken);

            var baseUri = System.Configuration.ConfigurationManager.AppSettings["WebApiUrl"];
            httpClient.BaseAddress = new Uri(baseUri);

            var response = await httpClient.DeleteAsync(string.Concat("/api/ac/", actionName));

            return response;
        }

        public static async Task<HttpResponseMessage> GetWebApi<T>(string actionName, int? id = null, string bearerToken = null) where T : class, IModel
        {
            if (actionName == null)
                throw new ArgumentNullException("actionName");

            if (!string.IsNullOrEmpty(bearerToken) && bearerToken.StartsWith(BEARER))
                bearerToken = string.Concat(BEARER, bearerToken);

            var httpClient = HttpClientFactory.Create();
            if (!string.IsNullOrEmpty(bearerToken))
                httpClient.DefaultRequestHeaders.Add("Authorization", bearerToken);

            var baseUri = System.Configuration.ConfigurationManager.AppSettings["WebApiUrl"];
            httpClient.BaseAddress = new Uri(baseUri);

            var formatUri = "/api/ac/{0}";
            if (id.HasValue)
                formatUri = string.Concat(formatUri, "/{1}");

            var response = await httpClient.GetAsync(string.Format(formatUri, actionName, id.GetValueOrDefault(0)));

            return response;
        }
    }
}
