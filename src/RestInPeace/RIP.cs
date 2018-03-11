namespace RestInPeace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class RIP : IArrangeContext, IActContext, IAssertContext, IExecutionContext
    {
        private HttpClient _httpClient;
        private Uri _baseAddress;
        private string _requestUri;
        private HttpMethod _httpMethod;
        private HttpContent _body;
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _queryStrings = new Dictionary<string, string>();

        RIP() { }

        #region IArrangeContext

        public static IArrangeContext Given() => new RIP();

        IActContext IArrangeContext.When() => this as IActContext;

        IArrangeContext IArrangeContext.Body(object body)
        {
            if (body == null)
            {
                return null;
            }

            string stringPayload = JsonConvert.SerializeObject(body);
            _body = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            return this;
        }

        IArrangeContext IArrangeContext.Header(string key, string value)
        {
            if (!_headers.ContainsKey(key))
            {
                _headers.Add(key, value);
            }

            return this;
        }

        IArrangeContext IArrangeContext.Headers(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                if (!_headers.ContainsKey(header.Key))
                {
                    _headers.Add(header.Key, header.Value);
                }
            }

            return this;
        }

        IArrangeContext IArrangeContext.Query(string key, string value)
        {
            if (!_queryStrings.ContainsKey(key))
            {
                _queryStrings.Add(key, value);
            }

            return this;
        }

        IArrangeContext IArrangeContext.Queries(Dictionary<string, string> queries)
        {
            foreach (var query in queries)
            {
                if (!_queryStrings.ContainsKey(query.Key))
                {
                    _queryStrings.Add(query.Key, query.Value);
                }
            }

            return this;
        }

        IArrangeContext IArrangeContext.HttpClient(HttpClient client)
        {
            _httpClient = client;
            return this;
        }

        IArrangeContext IArrangeContext.BaseAddress(Uri uri)
        {
            _baseAddress = uri;
            return this;
        }

        IArrangeContext IArrangeContext.BaseAddress(string uri)
        {
            _baseAddress = uri.EndsWith("/") ? new Uri(uri)
                                             : new Uri(uri + "/");
            return this;
        }

        #endregion

        #region IActContext

        IAssertContext IActContext.Get(string requestUri)
        {
            _httpMethod = HttpMethod.Get;
            _requestUri = requestUri;
            return this;
        }

        IAssertContext IActContext.Post(string requestUri)
        {
            _httpMethod = HttpMethod.Post;
            _requestUri = requestUri;
            return this;
        }

        IAssertContext IActContext.Put(string requestUri)
        {
            _httpMethod = HttpMethod.Put;
            _requestUri = requestUri;
            return this;
        }

        IAssertContext IActContext.Patch(string requestUri)
        {
            _httpMethod = HttpMethod.Patch;
            _requestUri = requestUri;
            return this;
        }

        IAssertContext IActContext.Delete(string requestUri)
        {
            _httpMethod = HttpMethod.Delete;
            _requestUri = requestUri;
            return this;
        }

        #endregion

        #region IAssertContext

        IExecutionContext IAssertContext.Then() => this as IExecutionContext;

        #endregion 

        #region IExecutionContext

        void IExecutionContext.AssertThat(Func<HttpResponse, bool> predicate) => predicate(Execute());

        HttpResponse IExecutionContext.Retrieve() => Execute();

        private HttpResponse Execute()
        {
            if (_httpClient == null)
            {
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };

                _httpClient = new HttpClient(handler, true);

                if (_baseAddress != null)
                {
                    _httpClient.BaseAddress = _baseAddress;
                }

                foreach (var kvp in _headers)
                {
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(kvp.Key, kvp.Value); // naïve way of adding headers
                }
            }

            if (_queryStrings.Any())
            {
                _requestUri += string.Format("?{0}", string.Join("&", _queryStrings.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))));
            }

            switch (_httpMethod)
            {
                case HttpMethod.Get:
                    return new HttpResponse(_httpClient.GetAsync(_requestUri).Result);
                case HttpMethod.Post:
                    return new HttpResponse(_httpClient.PostAsync(_requestUri, _body).Result);
                case HttpMethod.Put:
                    return new HttpResponse(_httpClient.PutAsync(_requestUri, _body).Result);
                case HttpMethod.Patch:
                    return new HttpResponse(PatchAsync(_httpClient, _requestUri, _body).Result);
                case HttpMethod.Delete:
                    return new HttpResponse(_httpClient.DeleteAsync(_requestUri).Result);
                default:
                    throw new NotSupportedException();
            }
        }

        private static async Task<HttpResponseMessage> PatchAsync(HttpClient client, string requestUri, HttpContent content)
        {
            var request = new HttpRequestMessage(new System.Net.Http.HttpMethod("PATCH"), requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request);
        }

        #endregion

        public class HttpResponse
        {
            internal HttpResponse(HttpResponseMessage httpResponseMessage)
            {
                HttpStatusCode = httpResponseMessage.StatusCode;
                Headers = httpResponseMessage.Content.Headers.ToDictionary(x => x.Key.Trim(), x => x.Value);
                Content = httpResponseMessage.Content.ReadAsStringAsync().Result;
            }

            public HttpStatusCode HttpStatusCode { get; }
            public Dictionary<string, IEnumerable<string>> Headers { get; }
            public string Content { get; }
            public T GetContent<T>() => JsonConvert.DeserializeObject<T>(Content);
        }
    }
        
    public interface IArrangeContext
    {
        IActContext When();
        IArrangeContext Body(object body);
        IArrangeContext Header(string key, string value);
        IArrangeContext Headers(Dictionary<string, string> headers);
        IArrangeContext Queries(Dictionary<string, string> queries);
        IArrangeContext Query(string key, string value);
        IArrangeContext HttpClient(HttpClient client);
        IArrangeContext BaseAddress(Uri uri);
        IArrangeContext BaseAddress(string uri);
    }

    public interface IActContext
    {
        IAssertContext Get(string requestUri);
        IAssertContext Post(string requestUri);
        IAssertContext Put(string requestUri);
        IAssertContext Patch(string requestUri);
        IAssertContext Delete(string requestUri);
    }

    public interface IAssertContext
    {
        IExecutionContext Then();
    }

    public interface IExecutionContext
    {
        void AssertThat(Func<RIP.HttpResponse, bool> predicate);
        RIP.HttpResponse Retrieve();
    }

    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }
}
