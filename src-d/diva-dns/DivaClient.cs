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
        public async Task<HttpClient> SendGetRequestAsync(string url, string Requestinfo) // han jetzt mal mit webrequest gaschafet aber mer chans ja au eifach ändere
        {
            Console.WriteLine("You startet get function");
            string GetRequestInfo = url + Requestinfo;
            // Create a new request to the specified URL
            HttpResponseMessage response = await _client.GetAsync(GetRequestInfo);

            // Send the request and get the response
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            else if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Something went wrong."); //Displays an error message in the Console
                System.Environment.Exit(1); //Stops the programm
            }
            return null;
        }
        //Simple Put Request function
        public async Task<HttpClient> SendPutRequestAsync(string url, string DomainName, string Ip)
        {
            Console.WriteLine("You startet Put function");
            // Create a new request to the specified URL
            string requestBody = url + DomainName + "/" + Ip;
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.IsSuccessStatusCode);
            }
            return null;
        }
    }
}
