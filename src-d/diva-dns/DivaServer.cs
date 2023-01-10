using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace diva_dns
{
    public class SearchResult
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class AboutResult
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("license")]
        public string License { get; set; }

        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class Transaction
    {
        [JsonPropertyName("seq")]
        public int Sequence { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("ns")]
        public string Namespace { get; set; }

        [JsonPropertyName("h")]
        public int BlockChainHeight { get; set; }

        [JsonPropertyName("d")]
        public string Data { get; set; }

        public static Transaction Create(string domainName, int blockChainHeight, string b32)
        {
            return new Transaction()
            {
                Sequence = 1,
                Command = "decision",
                Namespace = $"I2PNS:{domainName}",
                BlockChainHeight = blockChainHeight + 25,
                Data = $"domain-name={b32}"
            };
        }
    }

    public class Data
    {
        [JsonPropertyName("seq")]
        public int Sequence { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("ns")]
        public string Namespace { get; set; }

        [JsonPropertyName("d")]
        public string Content { get; set; }

        public static Data Create(string domainName, string b32)
        {
            return new Data()
            {
                Sequence = 1,
                Command = "data",
                Namespace = $"IIPDNS:{domainName}",
                Content = b32
            };
        }

        public bool IsValid()
        {
            Regex nsMatcher = new Regex(@"^([A-Za-z_-]{4,15}:){1,4}[A-Za-z0-9_-]{1,64}$");
            return Sequence >= 1 && Command == "data" && nsMatcher.IsMatch(Namespace) && Content.Length <= 8192;
        }
    }

    /// <summary>
    /// Simple get request. Send and wait with timeout.
    /// </summary>
    public class GetRequest
    {
        private readonly string _url;
        private readonly string _parameter;
        private readonly HttpClient _client;

        public HttpResponseMessage? ResponseMessage { get; private set; }

        public GetRequest(HttpClient client, string url, string getParameter = "")
        {
            _url = url;
            _parameter = getParameter;
            _client = client;
        }

        public bool SendAndWaitForAnswer(int timeout_in_ms = -1)
        {
            try
            {
                var url = _url + _parameter;
                var task = _client.GetAsync(url);
                if (task.Wait(timeout_in_ms))
                {
                    ResponseMessage = task.Result;
                    return true;
                }
            }
            catch (Exception e)
            {
                // Todo exception handling
            }

            return false;
        }
    }

    public class PutRequest
    {
        private readonly string _url;
        private readonly HttpClient _client;
        private readonly Data _data;

        public HttpResponseMessage? ResponseMessage { get; private set; }

        public PutRequest(HttpClient client, string url, Data data)
        {
            _client = client;
            _url = url;
            _data = data;
        }

        public bool SendAndWaitForAnswer(int timeout_in_ms = -1)
        {
            try
            {
                var task = _client.PutAsJsonAsync(_url + "transaction/", new[] { _data });
                if(task.Wait(timeout_in_ms))
                {
                    ResponseMessage = task.Result;
                    return true;
                }
            } catch (Exception e)
            {
                // Todo exception handling
            }

            return false;
        }
    }

    /// <summary>
    /// Simple post request for a Transaction. Send and wait for response with timeout.
    /// </summary>
    public class PostRequest
    {
        private readonly string _url;
        private readonly HttpClient _client;
        private readonly Transaction _transaction;

        public HttpResponseMessage? ResponseMessage { get; private set; }

        public PostRequest(HttpClient client, string url, Transaction transaction)
        {
            _client = client;
            _url = url;
            _transaction = transaction;
        }

        public bool SendAndWaitForReponse(int timeout_in_ms = -1)
        {
            try
            {
                var task = _client.PostAsJsonAsync(_url + "transaction/", _transaction);
                if (task.Wait(timeout_in_ms))
                {
                    ResponseMessage = task.Result;
                    return true;
                }
            }
            catch (Exception e)
            {
                // Todo exception handling
            }

            return false;
        }
    }

    public class DivaServer
    {
        private readonly HttpListener _listener = new();
        private readonly HttpClient _client = new();

        private readonly string _listeningPrefix;
        private readonly string _localDivaAddress;

        /// <summary>
        /// Helper to convert to a diva v34 domain name from a current domain name.
        /// Workaround received from Samuel Abaecherli, Pascal Knecht
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        private string ConvertToV34(string domainName)
        {
            return domainName.Replace(".i2p", ":i2p_");
        }

        /// <summary>
        /// Helper to convert a diva v34 domain name to a current domain name.
        /// Workaround received from Samuel Abaecherli, Pascal Knecht
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        private string ConvertFromV34(string domainName)
        {
            return domainName.Replace(":i2p_", ".i2p");
        }

        /// <summary>
        /// Fetch the height of the blockChain.
        /// </summary>
        /// <returns></returns>
        private int GetHeight()
        {
            var request = new GetRequest(_client, _localDivaAddress, "about");
            if (request.SendAndWaitForAnswer())
            {
                var response = request.ResponseMessage;
                if (!response?.IsSuccessStatusCode ?? true)
                {
                    return -1;  // return -1 in case of error
                }

                var contentStream = response.Content.ReadAsStream();
                var about = JsonSerializer.Deserialize<AboutResult>(contentStream);

                return about.Height;
            }

            return -1;
        }

        /// <summary>
        /// Search with a query and return the first obtained result
        /// </summary>
        /// <param name="query"></param>
        /// <param name="searchResult"></param>
        /// <returns></returns>
        public HttpStatusCode PerformSearchQuery(string query, out SearchResult? searchResult)
        {
            searchResult = null;
            var request = new GetRequest(_client, _localDivaAddress, query);
            if(request.SendAndWaitForAnswer())
            {
                if(request.ResponseMessage?.IsSuccessStatusCode ?? false)
                {
                    var contentStream = request.ResponseMessage.Content.ReadAsStream();
                    var results = JsonSerializer.Deserialize<SearchResult[]>(contentStream);

                    searchResult = results[0];
                    return request.ResponseMessage.StatusCode;
                }

                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.ServiceUnavailable;
        }

        /// <summary>
        /// Put a new dns entry
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="b32"></param>
        /// <returns></returns>
        public HttpStatusCode PerformPutRequest(string domainName, string b32)
        {
            var v34DomainName = ConvertToV34(domainName);
            var data = Data.Create(v34DomainName, b32);
            var request = new PutRequest(_client, _localDivaAddress, data);
            if(request.SendAndWaitForAnswer() && request.ResponseMessage != null)
            {
                return request.ResponseMessage.StatusCode;
            }

            return HttpStatusCode.ServiceUnavailable;

        }

        public DivaServer(string prefix, string localDivaAddress)
        {
            _listeningPrefix = prefix;
            _localDivaAddress = localDivaAddress;

            _listener.Prefixes.Add(prefix);
        }

        /// <summary>
        /// Handle a get or put/post message incoming from DivaClient.
        /// Forward it to Diva and report the response back to DivaClient.
        /// </summary>
        public void HandleMessage()
        {
            var context = _listener.GetContext();
            var request = context.Request;
            var response = context.Response;
            byte[] data = new byte[0];

            switch (request.HttpMethod)
            {
                case "GET":
                    {
                        if (InputValidation.IsProperGetArgument(request.RawUrl ?? string.Empty))
                        {
                            var parameter = request.RawUrl[1..]; // cut off initial '/'
                            var statusCode = PerformSearchQuery(parameter, out SearchResult? searchResult);

                            response.StatusCode = (int)statusCode;
                            response.ContentLength64 = 0;
                            if (searchResult != null)
                            {
                                var json = JsonSerializer.Serialize(searchResult);
                                data = Encoding.UTF8.GetBytes(json);
                            }                            
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        break;
                    }
                case "PUT":
                    {
                        if (InputValidation.IsProperPutArgument(request.RawUrl ?? string.Empty))
                        {
                            var parameter = request.RawUrl[1..]; // cut off initial '/'
                            var parts = parameter.Split('/');
                            var statusCode = PerformPutRequest(parts[0], parts[1]);
                            response.StatusCode = (int)statusCode;
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        break;
                    }
                default:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
            }

            var outStream = response.OutputStream;
            outStream.Write(data, 0, data.Length);
            outStream.Close();
        }

        public void Start()
        {
            _listener.Start();

            // Todo: Run HandleMessage in a loop and exit, when server is stopped

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        HandleMessage();
            //    }
            //});
        }

        public void Stop()
        {
            Thread.Sleep(1000);
            _listener.Stop();
        }

        public bool IsConnected()
        {
            var request = new GetRequest(_client, _localDivaAddress, "about");
            return request.SendAndWaitForAnswer(300);
        }
    }
}
