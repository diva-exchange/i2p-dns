﻿using diva_dns.Data;
using diva_dns.Requests;
using diva_dns.Util;
using System.Net;
using System.Text;
using System.Text.Json;

namespace diva_dns
{
    public class DivaDnsServer
    {
        private readonly HttpListener _listener = new();
        private readonly HttpClient _client = new();

        private readonly string _localDivaAddress;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private bool _isWaitingForMessage = false;

        private Task _messageWorker;

        /// <summary>
        /// Search with a query and return the first obtained result
        /// </summary>
        /// <param name="query"></param>
        /// <param name="searchResult"></param>
        /// <returns></returns>
        public Result<string> ResolveDomainName(string domainName)
        {
            domainName = domainName.ConvertToV34().AddI2pPrefix();
            if (!InputValidation.IsProperNsV34(domainName))
            {
                return new Result<string> { StatusCode = HttpStatusCode.BadRequest, Value = null };
            }
            var query = $"state/search/{domainName}";

            var request = new GetRequest(_client, _localDivaAddress, query);
            if (request.SendAndWaitForAnswer())
            {
                if (request.ResponseMessage?.IsSuccessStatusCode ?? false)
                {
                    var contentStream = request.ResponseMessage.Content.ReadAsStream();
                    var results = JsonSerializer.Deserialize<SearchResult[]>(contentStream);

                    if (results is not null && results.Any())
                    {
                        return new Result<string> { StatusCode = request.ResponseMessage.StatusCode, Value = results[0].Value.ConvertFromV34() };
                    }
                    return new Result<string> { StatusCode = request.ResponseMessage.StatusCode, Value = null };
                }

                return new Result<string> { StatusCode = HttpStatusCode.InternalServerError, Value = null };
            }
            return new Result<string> { StatusCode = HttpStatusCode.ServiceUnavailable, Value = null };
        }

        /// <summary>
        /// Put a new dns entry
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="b32Address"></param>
        /// <returns></returns>
        public Result RegisterDomainName(string domainName, string b32Address)
        {
            domainName = domainName.ConvertToV34().AddI2pPrefix();

            if (!InputValidation.IsProperNsV34(domainName) || !InputValidation.IsB32String(b32Address))
            {
                return new Result { StatusCode = HttpStatusCode.BadRequest };
            }

            var data = TransactionV34.PutDomainName(domainName, b32Address);
            var request = new PutRequest(_client, _localDivaAddress, data);

            if (request.SendAndWaitForAnswer() && request.ResponseMessage != null)
            {
                return new Result { StatusCode = request.ResponseMessage.StatusCode };
            }

            return new Result { StatusCode = HttpStatusCode.ServiceUnavailable };

        }

        public DivaDnsServer(string prefix, string localDivaAddress)
        {
            _localDivaAddress = localDivaAddress;

            _listener.Prefixes.Add(prefix);
        }

        /// <summary>
        /// Handle a get or put/post message incoming from DivaClient.
        /// Forward it to Diva and report the response back to DivaClient.
        /// </summary>
        public void HandleMessage()
        {
            try
            {
                _isWaitingForMessage = true;
                var context = _listener.GetContext();
                _isWaitingForMessage = false;
                var request = context.Request;
                var response = context.Response;
                byte[] data = new byte[0];

                switch (request.HttpMethod)
                {
                    case "GET":
                        {
                            var parameter = request.RawUrl?.Length > 0 ? request.RawUrl[1..] : "/";  // cut off initial '/'
                            var result = ResolveDomainName(parameter);

                            response.StatusCode = (int)result.StatusCode;
                            response.ContentLength64 = 0;
                            if (result.Value != null)
                            {
                                var json = JsonSerializer.Serialize(new ResolveDomainNameResult { B32Address = result.Value });
                                data = Encoding.UTF8.GetBytes(json);
                            }
                            break;
                        }
                    case "PUT":
                        {
                            var parameter = request.RawUrl?.Length > 0 ? request.RawUrl[1..] : "/"; // cut off initial '/'
                            var parts = parameter.Split('/');
                            var result = RegisterDomainName(parts[0], parts[1]);
                            response.StatusCode = (int)result.StatusCode;
                            break;
                        }
                    default:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                }

                var outStream = response.OutputStream;
                outStream.Write(data, 0, data.Length);
                outStream.Close();
            } catch
            {
                // we need the try catch because GetContext() does not unblock when the HttpListener is stopped.
            }
        }

        public void Start()
        {
            var token = _tokenSource.Token;
            _listener.Start();

            _messageWorker = Task.Factory.StartNew(() =>
            {
                while(!token.IsCancellationRequested)
                {
                    HandleMessage();
                }
            }, token);          
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            Thread.Sleep(200);
            if (_messageWorker.IsCanceled)
            {
                _listener.Stop();
            } else
            {
                _listener.Abort();
            }
            _messageWorker.Dispose();
        }

        public bool IsConnected()
        {
            var request = new GetRequest(_client, _localDivaAddress, "about");
            return request.SendAndWaitForAnswer(300);
        }
    }

    public class Result
    {
        public HttpStatusCode StatusCode { get; set; }
    }

    public class Result<TValue> : Result
    {
        public TValue? Value { get; set; }
    }
}