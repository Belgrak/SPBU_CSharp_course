using System.Net;

public class ServerCLI
{
    public static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Please check arguments. Server ip and port were expected");
            return;
        }

        if (!IPAddress.TryParse(args[0], out IPAddress? ip))
        {
            Console.WriteLine("Incorrect ip");
            return;
        }

        if (!int.TryParse(args[1], out int port))
        {
            Console.WriteLine("Incorrect port");
            return;
        }

        var server = new Server.Server(ip, port);
        await server.StartListen();
    }
}