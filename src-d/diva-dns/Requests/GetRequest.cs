namespace diva_dns.Requests
{
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
}
