using Newtonsoft.Json;
using SH.Web.Infrastructure.RetryPolicy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SH.Web.Services
{
    public class BaseApiService
    {
        public IPrincipal User { get; set; }

        public IRetryPolicy RetryPolicy { get; set; } = new HttpClientRetryPolicy();

        private string baseurl = ConfigurationManager.AppSettings["webapi_baseurl"];

        protected async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (HttpClient httpClient = new HttpClient())
            {
                AttachBearerAccessToken(httpClient);

                HttpResponseMessage response = null;

                 await RetryPolicy.RetryAsync(async () =>
                 {
                     response = await httpClient.GetAsync(baseurl + url);
                     return response;
                 }, cancellationToken).ConfigureAwait(false);

#if DEBUG
                string responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseString);
#else
                    return await response.Content.ReadAsAsync<T>();
#endif

            }
            throw new InvalidOperationException();
        }

        protected async Task<T> PostAsync<T>(string url, T value, MediaTypeFormatter formatter = null)
        {
            string responseString = null;
            using (HttpClient httpClient = new HttpClient())
            {
                AttachBearerAccessToken(httpClient);

                if (formatter is BsonMediaTypeFormatter)
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/bson"));
                }

                var response = await httpClient.PostAsync(baseurl + url, value, formatter ?? new JsonMediaTypeFormatter());
                if (response != null)
                {
                    responseString = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK && string.IsNullOrEmpty(responseString))
                    {
                        return await Task.FromResult(default(T));
                    }
                }
            }
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        protected async Task DeleteAsync(string url)
        {
            string responseString = null;
            using (HttpClient httpClient = new HttpClient())
            {
                AttachBearerAccessToken(httpClient);

                var response = await httpClient.DeleteAsync(baseurl + url);
                if (response != null)
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        throw new InvalidOperationException(responseString);
                    }
                }
            }
        }

        private void AttachBearerAccessToken(HttpClient httpClient)
        {
            if (User != null && User is ClaimsPrincipal && User.Identity.IsAuthenticated)
            {
                var jwt = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "access_token").Value;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }
        }
    }
}