using diva_dns.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace diva_dns.Requests
{
    /// <summary>
    /// Simple post request for a Transaction. Send and wait for response with timeout.
    /// </summary>
    public class PostRequest : IRequest
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

        public bool SendAndWaitForAnswer(int timeout_in_ms = -1)
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
}
