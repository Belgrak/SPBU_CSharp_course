using System.Net;

public class ClientCLI
{
    public static void Main(string[] args)
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

        var client = new Client.Client(ip, port);

        Console.WriteLine("Commands: ");
        Console.WriteLine("1 {path} --- returns size of directory and list of files and directories");
        Console.WriteLine("2 {path} --- returns size of file and content");
        Console.WriteLine("exit --- stops the client");

        while (true)
        {
            var stringData = Console.ReadLine();
            switch (stringData)
            {
                case "exit":
                    return;
                case null:
                    Console.WriteLine("Command not found");
                    continue;
            }

            var listOfArguments = stringData.Split(' ');
            if (listOfArguments.Length != 2)
            {
                Console.WriteLine("Incorrect input");
                continue;
            }

            switch (listOfArguments[0])
            {
                case "1":
                {
                    try
                    {
                        var (size, strings) = client.List(listOfArguments[1]).Result;
                        Console.WriteLine($"Elements count: {size}");
                        for (var i = 1; i <= 2 * size; i += 2)
                        {
                            Console.WriteLine($"{i}): {strings[i - 1]} is dir: {strings[i]}");
                        }
                    }
                    catch (DirectoryNotFoundException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }

                    break;
                }
                case "2":
                {
                    try
                    {
                        var answer = client.Get(listOfArguments[1]).Result;
                        Console.WriteLine($"File size: {answer.Item1}");
                        Console.WriteLine($"Content: {answer.Item2}");
                    }
                    catch (DirectoryNotFoundException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }

                    break;
                }
                default:
                {
                    Console.WriteLine("Command not found");
                    continue;
                }
            }
        }
    }
}