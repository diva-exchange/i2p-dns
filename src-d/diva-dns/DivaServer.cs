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
using System.Threading.Tasks;

namespace diva_dns
{
    public struct Transaction
    {
        int seq;
        string command;
        string ns;
        int h;
        string d;

        public static Transaction Create(string domainName, int blockChainHeight, string b32)
        {
            return new Transaction()
            {
                seq = 1,
                command = "decision",
                ns = $"I2PNS:{domainName}",
                h = blockChainHeight + 25,
                d = $"domain-name={b32}"
            };
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

        private int _blockChainHeight = -1;

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
                var json = JsonDocument.Parse(contentStream);

                if (json.RootElement.TryGetProperty("height", out JsonElement height))
                {
                    if (height.TryGetInt32(out int result))
                    {
                        return result;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Send a get request to Diva and return the response.
        /// </summary>
        /// <param name="rawUrl"></param>
        /// <returns></returns>
        public HttpResponseMessage HandleGetRequest(string rawUrl)
        {
            if (InputValidation.IsProperGetArgument(rawUrl))
            {
                string parameter = "state/decision:I2PDNS:" + rawUrl[1..];
                var request = new GetRequest(_client, _localDivaAddress, parameter);
                if (request.SendAndWaitForAnswer())
                {
                    if (request.ResponseMessage?.IsSuccessStatusCode ?? false)
                    {
                        return request.ResponseMessage;
                    }
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError); // forward error

                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable); // timeout
                }
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);  // return Bad Request
        }

        /// <summary>
        /// Send a Post/Transaction request to Diva and return the response.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="b32"></param>
        /// <returns></returns>
        public HttpResponseMessage HandlePostRequest(string domainName, string b32)
        {
            if (InputValidation.IsDomainName(domainName) && InputValidation.IsB32String(b32))
            {
                var height = GetHeight();
                if (height == -1)
                {
                    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                }

                var transaction = Transaction.Create(domainName, height, b32);
                var request = new PostRequest(_client, _localDivaAddress, transaction);
                if (request.SendAndWaitForReponse())
                {
                    if (request.ResponseMessage?.IsSuccessStatusCode ?? false)
                    {
                        return request.ResponseMessage;
                    }
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError); // forward error
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable); // timeout
                }
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest); // return bad request
        }

        private byte[] ToBuffer(string input)
        {
            return System.Text.Encoding.UTF8.GetBytes(input);
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
            string data = string.Empty;

            HttpResponseMessage divaResponse = null;

            switch (request.HttpMethod)
            {
                case "GET":
                    {
                        if (InputValidation.IsProperGetArgument(request.RawUrl ?? string.Empty))
                        {
                            var parameter = request.RawUrl[1..]; // cut off initial '/'
                            divaResponse = HandleGetRequest(parameter);
                            if (divaResponse.IsSuccessStatusCode)
                            {
                                // Todo: Parse resonse and send back with HttpListenerResponse
                            }
                        }
                        else
                        {
                            divaResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    }
                case "PUT":
                case "POST":
                    {
                        if (InputValidation.IsProperPutARgument(request.RawUrl ?? string.Empty))
                        {
                            var parameter = request.RawUrl[1..]; // cut off initial '/'
                            var parts = parameter.Split('/');
                            divaResponse = HandlePostRequest(parts[0], parts[1]);
                            if (divaResponse.IsSuccessStatusCode)
                            {
                                // Todo: Parse resonse and send back with HttpListenerResponse
                            }
                        }
                        else
                        {
                            divaResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    }
                default:
                    divaResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    break;
            }

            response.StatusCode = (int)divaResponse.StatusCode;
            var buffer = ToBuffer(data);
            var outStream = response.OutputStream;
            outStream.Write(buffer, 0, buffer.Length);
            outStream.Close();
        }

        public void Start()
        {
            _listener.Start();
            _blockChainHeight = GetHeight();

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
