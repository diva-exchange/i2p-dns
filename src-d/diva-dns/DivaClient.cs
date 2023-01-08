using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diva_dns
{
    public class DivaClient
    {
        private readonly HttpClient _client = new();  // do chasch ebbe au mit em HttpWebRequest schaffe, wenn dir dä lieber isch

        // Todo(siro) implement class that sends Get and Post/Put requests to DivaServer and returns the responses.

        /// <summary>
        /// Send a simple get-request and return the response
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(string url)
        {
            // Todo...  Example:
            var task = _client.GetAsync(url);
            task.Wait();
            return task.Result;
        }
    }
}
