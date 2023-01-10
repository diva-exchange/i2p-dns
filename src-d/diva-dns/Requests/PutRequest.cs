using System.Net.Http.Json;

namespace diva_dns.Requests
{
    public class PutRequest
    {
        private readonly string _url;
        private readonly HttpClient _client;
        private readonly Data.TransactionV34 _data;

        public HttpResponseMessage? ResponseMessage { get; private set; }

        public PutRequest(HttpClient client, string url, Data.TransactionV34 data)
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
