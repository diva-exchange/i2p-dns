

using diva_dns;

public class Program
{
    private static readonly DivaServer _server = new("http://localhost:8080/", "http://127.19.72.21:17468/");

    private static readonly DivaClient _client = new();

    static void Main(string[] args)
    {

        // Todo: Handle command line arguments
        // Possible arguments:
        // - Diva URL (IP + Port)
        // = Port for localhost server (currently 8080)
        DivaClient DivaClient = new DivaClient();

        Console.WriteLine("Hello Diva!");

        _server.Start();

        if (_server.IsConnected())
        {
            Console.WriteLine("Is connected to Diva");
        } else
        {
            Console.WriteLine("Has no connection to Diva");
        }

            //User input for Get or Post Request
            while (true)
            {
                string GetType = "get";
                string url = "http://localhost:8080/";
                string requestBody = "xxx";
                string PutType = "put";
                Console.WriteLine("Please select the Request type");
                Console.WriteLine("GET /[a-z0-9-_]{3-64}.i2p$");
                Console.WriteLine("PUT /[a-z0-9-_]{3-64}.i2p$/[a-z0-9]{52}$");
                Console.WriteLine("Write Get or Put");
                var Requesttype = Console.ReadLine();

                if (GetType.Equals(Requesttype, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("get");
                    Task<String> GetResponse = DivaClient.SendGetRequest(url);
                    Console.WriteLine(GetResponse);
                    break;
                }
                else if (PutType.Equals(Requesttype, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("put");
                    Task<String> PutResponse =  DivaClient.SendPutRequest(url, requestBody);
                    Console.WriteLine(PutResponse);
                    break;
                }
                else
                {
                    Console.WriteLine("This Request is invalid");
                }
            }
        // DivaServer will respond to diva client

        // Todo(siro) report responses from back to user
        // Todo(siro) handle input from console, when to terminate program and close server

        _server.Stop();

        Console.WriteLine("Bye, Diva!");
    }
}