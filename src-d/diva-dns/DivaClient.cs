using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace diva_dns
{
    public class DivaClient
    {
        private static readonly HttpClient _client = new();  // do chasch ebbe au mit em HttpWebRequest schaffe, wenn dir dä lieber isch

        // Todo(siro) implement class that sends Get and Post/Put requests to DivaServer and returns the responses.

        //Simple Get Request function
        public async Task SendGetRequestAsync(string url, string Requestinfo) // han jetzt mal mit webrequest gaschafet aber mer chans ja au eifach ändere
        {
            Console.WriteLine("[Get request]You startet get function");
            string GetRequestInfo = url + Requestinfo;
            // Create a new request to the specified URL
            HttpResponseMessage response = await _client.GetAsync(GetRequestInfo);

            // Send the request and get the response
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("[Get request]" + result);
            }
            else if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"[Get request]Not successful, status was {response.StatusCode}."); //Displays an error message in the Console
            }
            return;
        }
        //Simple Put Request function
        public async Task SendPutRequestAsync(string url, string DomainName, string Ip)
        {
            Console.WriteLine("[Put request]You startet Put function");
            // Create a new request to the specified URL
            string requestBody = url + DomainName + "/" + Ip;
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync(requestBody, content);

            Console.WriteLine($"[Put request]Received response with status {response.StatusCode}");

            return;
        }
    }
}
