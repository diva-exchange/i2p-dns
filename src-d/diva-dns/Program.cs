

using diva_dns;
using diva_dns.Util;

public class Program
{
    // default arguements
    private static string _divaChainAddress = "http://127.19.72.21:17468/";
    private static string _dnsServerAddress = "http://127.19.72.227:19445/";

    static void Main(string[] args)
    {

        // Environment variable supersede default arguments
        var envDiva = Environment.GetEnvironmentVariable("DIVA_DNS_DIVA_CHAIN_ADDRESS");
        if(!string.IsNullOrEmpty(envDiva))
        {
            _divaChainAddress = envDiva;
        }
        var envDns = Environment.GetEnvironmentVariable("DIVA_DNS_LOCAL_ADDRESS");
        if(!string.IsNullOrEmpty(envDns))
        {
            _dnsServerAddress = envDns;
        }


        // Command line arguments supersede Environment variables
        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i] == "--dns")
            {
                _dnsServerAddress = args[++i];
            } else if (args[i] == "--diva")
            {
                _divaChainAddress = args[++i];
            }
        }

        Console.WriteLine($"Address of Diva Chain set to: '{_divaChainAddress}'");
        Console.WriteLine($"Diva dns server listening on: '{_dnsServerAddress}'");

        var server = new DivaDnsServer(_dnsServerAddress, _divaChainAddress);

        // Todo: Handle command line arguments
        // Possible arguments:
        // - Diva URL (IP + Port)
        // = Port for localhost server (currently 8080)
        DivaClient DivaClient = new DivaClient();

        Console.WriteLine("Hello Diva!");

        server.Start();

        if (server.IsConnected())
        {
            Console.WriteLine("Is connected to Diva");
        }
        else
        {
            Console.WriteLine("Has no connection to Diva");
        }

            //User input for Get or Post Request
            while (true)
            {
            string GetType = "get";
            string PutType = "put";
            string Exit = "exit";
            string RequestInfo = null;
            string RequestIp = "";
            string RequestDomain = "";
            string url = "http://127.19.72.227:19445/";
                string requestBody = "/[a-z0-9-_]{3-64}\\.i2p$/[a-z0-9]{52}$";
                Console.WriteLine("Please select the Request type");
                Console.WriteLine("GET /[a-z0-9-_]{3-64}.i2p$");
                Console.WriteLine("PUT /[a-z0-9-_]{3-64}.i2p$/[a-z0-9]{52}$");
                Console.WriteLine("Write Get or Put");
                var Requesttype = Console.ReadLine();
            //split first word check get put exit, if more then one word check

            string[] splitted_string = Requesttype.Split(" ");
            if (splitted_string.Length > 1)
            {
                switch(splitted_string.Length)
                     { case 1: 
                        {
                            Requesttype = splitted_string[0];
                            break;
                        }
                    case 2:
                        {
                            Requesttype = splitted_string[0];
                            RequestDomain = splitted_string[1];
                            break;
                        }
                    default:
                        {
                            Requesttype = splitted_string[0];
                            RequestDomain = splitted_string[1];
                            RequestIp = splitted_string[2];
                            break;
                        }
                }
            }

                if (GetType.Equals(Requesttype, StringComparison.OrdinalIgnoreCase))
                {
                    var GetResponse = DivaClient.SendGetRequestAsync(url, RequestDomain ?? string.Empty);
                }
                else if (PutType.Equals(Requesttype, StringComparison.OrdinalIgnoreCase))
                {
                    var PutResponse = DivaClient.SendPutRequestAsync(url, RequestDomain ?? string.Empty, RequestIp ?? string.Empty);
                    Console.WriteLine(PutResponse);
                }
                else if (Exit.Equals(Requesttype, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exit");
                    break;
                }

                else
                {
                    Console.WriteLine("This Request is invalid");
                }
            }

        server.Stop();

        Console.WriteLine("Bye, Diva!");
    }
}