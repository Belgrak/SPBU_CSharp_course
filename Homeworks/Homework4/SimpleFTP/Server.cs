using System.Net;
using System.Net.Sockets;

namespace SimpleFTP;

public class Server
{
    private readonly IPAddress _ipAddress;
    private readonly int _port;

    public Server(IPAddress ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    private static async void List(NetworkStream stream, string path)
    {
        var writer = new StreamWriter(stream);
        if (!Directory.Exists(path))
        {
            await writer.WriteAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);

        await writer.WriteAsync((directories.Length + files.Length).ToString());
        await writer.FlushAsync();

        foreach (var file in files)
        {
            Console.WriteLine(file);
            await writer.WriteAsync($" {file} false");
            await writer.FlushAsync();
        }

        foreach (var directory in directories)
        {
            await writer.WriteAsync($" {directory} true");
            await writer.FlushAsync();
        }
    }

    private static async void Get(NetworkStream stream, string path)
    {
        var writer = new StreamWriter(stream);

        if (!File.Exists(path))
        {
            await writer.WriteAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var file = new FileInfo(path);

        await writer.WriteLineAsync(BitConverter.GetBytes(file.Length).Length.ToString());
        await writer.FlushAsync();

        await using var fileStream = file.Open(FileMode.Open);

        await fileStream.CopyToAsync(writer.BaseStream);
        await writer.FlushAsync();
    }

    public async void StartListen()
    {
        var listener = new TcpListener(_ipAddress, _port);
        listener.Start();
        Console.WriteLine("I'm listening");
        while (true)
        {
            using var socket = listener.AcceptSocket();
            await using var stream = new NetworkStream(socket);
            using var reader = new StreamReader(stream);
            var data = await reader.ReadLineAsync();

            var listOfArguments = (data)?.Split(' ');
            if (listOfArguments is not { Length: 2 })
            {
                Console.WriteLine("Incorrect request");
                continue;
            }

            if (listOfArguments[0] == "1")
            {
                Console.WriteLine("list");
                await Task.Run(() => List(stream, listOfArguments[1]));
            }

            if (listOfArguments[0] == "2")
            {
                Console.WriteLine("get");
                await Task.Run(() => Get(stream, listOfArguments[1]));
            }
        }

        listener.Stop();
    }
}