using diva_dns.Data;
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
            if (request.SendAndWaitForAnswer(2000))
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

            if (request.SendAndWaitForAnswer(2000) && request.ResponseMessage != null)
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
        public void HandleMessage(IAsyncResult asyncState)
        {
            HttpListenerRequest? request = null;
            HttpListenerResponse? response = null;
            HttpListenerContext? context = null;
            HttpListener listener = (HttpListener)asyncState.AsyncState;

            try
            {
                context = listener.EndGetContext(asyncState);
                request = context.Request;
                response = context.Response;
                byte[] data = new byte[0];


                switch (request.HttpMethod)
                {
                    case "GET":
                        {
                            var parameter = request.RawUrl?.Length > 0 ? request.RawUrl[1..] : "/";  // cut off initial '/'
                            Console.WriteLine($"[Diva DNS Info] Received request: GET/{parameter}");
                            var result = ResolveDomainName(parameter);

                            if (result.Value != null)
                            {
                                Console.WriteLine($"[Diva DNS Info] Asked diva about '{parameter}'. Received response with status={result.StatusCode} and Value='{result.Value}'");
                                var json = JsonSerializer.Serialize(new ResolveDomainNameResult { B32Address = result.Value });
                                data = Encoding.UTF8.GetBytes(json);
                                response.StatusCode = (int)result.StatusCode;
                            }
                            if (string.IsNullOrEmpty(result.Value))
                            {
                                Console.WriteLine($"[Diva DNS Info] Asked Diva about '{parameter}'. Received empty response with status={result.StatusCode} - will respond with 404");
                                response.StatusCode = (int)HttpStatusCode.NotFound;
                            }
                            break;
                        }
                    case "PUT":
                        {
                            var parameter = request.RawUrl?.Length > 0 ? request.RawUrl[1..] : "/"; // cut off initial '/'
                            var parts = parameter.Split('/');
                            Console.WriteLine($"[Diva DNS Info] Received request: PUT/{parameter}");
                            var result = RegisterDomainName(parts[0], parts[1]);
                            Console.WriteLine($"[Diva DNS Info] Asked diva to put '{parameter}'. Received response with status={result.StatusCode}");
                            response.StatusCode = (int)result.StatusCode;
                            break;
                        }
                    default:
                        Console.WriteLine("[Diva DNS WArning] Got unhandlend request (neither get nor put)");
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                }

                response.ContentLength64 = data.Length;
                var outStream = response.OutputStream;
                outStream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                // we need the try catch because GetContext() does not unblock when the HttpListener is stopped.
                Console.WriteLine($"[Diva DNS Warning] Caught exception: {e.Message} --- Full Exception: {e}");
            }
            finally
            {
                // even in case of error, we still need to send the response
                response?.OutputStream.Close();
                response?.Close();
                Console.WriteLine("[Diva DNS Info] Sent response to request");
            }
        }

        public void Start()
        {
            var token = _tokenSource.Token;
            _listener.Start();

            _messageWorker = Task.Factory.StartNew(() =>
            {
                bool keepRunning = true;
                while (keepRunning)
                {
                    IAsyncResult asyncResult = _listener.BeginGetContext(new AsyncCallback(HandleMessage), _listener);
                    int taskIndex = WaitHandle.WaitAny(new[]
                    {
                        asyncResult.AsyncWaitHandle, token.WaitHandle
                    });
                    switch (taskIndex)
                    {
                        case 0:
                            break;
                        case 1:
                            keepRunning = false;
                            break;
                        default:
                            token.ThrowIfCancellationRequested();
                            break;
                    }
                }
            });
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            Thread.Sleep(400);
            if (_messageWorker.IsCanceled)
            {
                _listener.Stop();
            }
            else
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
