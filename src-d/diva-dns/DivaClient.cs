using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diva_dns
{
    public class DivaClient
    {
        private static readonly HttpClient _client = new();  // do chasch ebbe au mit em HttpWebRequest schaffe, wenn dir dä lieber isch

        // Todo(siro) implement class that sends Get and Post/Put requests to DivaServer and returns the responses.

        //Simple Get Request function
        public HttpWebRequest SendGetRequest(string url) // han jetzt mal mit webrequest gaschafet aber mer chans ja au eifach ändere
        {
            Console.WriteLine("You startet get function");
            // Create a new request to the specified URL
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // Set the method to GET
            request.Method = "GET";

            // Send the request and get the response
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong."); //Displays an error message in the Console
                System.Environment.Exit(1); //Stops the programm
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Something went wrong."); //Displays an error message in the Console
                System.Environment.Exit(1); //Stops the programm
            }

            // Get the response stream
            System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream());

            // Read the response and return it as a string
            return streamReader.ReadToEnd();
        }
        //Simple Put Request function
        public HttpWebRequest SendPutRequest(string url, string requestBody)
        {
            Console.WriteLine("You startet Post function");
            // Create a new request to the specified URL
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // Set the method to POST
            request.Method = "POST";

            // Set the content type and content length
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] requestData = System.Text.Encoding.UTF8.GetBytes(requestBody);
            request.ContentLength = requestData.Length;

            // Write the request body
            System.IO.Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // Send the request and get the response
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong."); //Displays an error message in the Console
                System.Environment.Exit(1); //Stops the programm
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Something went wrong."); //Displays an error message in the Console
                System.Environment.Exit(1); //Stops the programm
            }
            

            // Get the response stream
            System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream());
            Console.WriteLine(request);
            // Read the response and return it as a string
            return streamReader.ReadToEnd();
        }
    }
}
