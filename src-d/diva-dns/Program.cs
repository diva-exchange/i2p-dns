

using diva_dns;

public class Program
{
    private static readonly DivaServer _server = new();

    private static readonly DivaClient _client = new();

    static void Main(string[] args)
    {
        Console.WriteLine("Hello Diva!");

        _server.Start();

        // Todo(siro) handle input from console and forward get/post to DivaClient
        // Todo(siro) report responses back to user
        // Todo(siro) hanlde input from console, when to terminate program and close server

        _server.Stop();
        
        Console.WriteLine("Bye, Diva!");
    }
}