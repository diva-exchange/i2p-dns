using diva_dns.Requests;
using diva_dns.Data;
using diva_dns.Util;
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
using diva_dns.Util;

namespace diva_dns
{
    public class DivaServer
    {
        private readonly HttpListener _listener = new();
        private readonly HttpClient _client = new();

        private readonly string _listeningPrefix;
        private readonly string _localDivaAddress;

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
            var v34DomainName = domainName.ConvertToV34(); ;
            var data = Data.Data.Create(v34DomainName, b32);
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
