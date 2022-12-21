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
        using var streamWriter = new StreamWriter(stream);
        if (!Directory.Exists(path))
        {
            await streamWriter.WriteAsync("-1");
            await streamWriter.FlushAsync();
            return;
        }

        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        var size = directories.Length + files.Length;

        await streamWriter.WriteAsync(size.ToString());
        await streamWriter.FlushAsync();

        foreach (var file in files)
        {
            await streamWriter.WriteAsync($" {file} false");
            await streamWriter.FlushAsync();
        }

        foreach (var directory in directories)
        {
            await streamWriter.WriteAsync($" {directory} true");
            await streamWriter.FlushAsync();
        }
        
        // var writer = new StreamWriter(stream);
        // if (!Directory.Exists(path))
        // {
        //     await writer.WriteAsync("-1");
        //     await writer.FlushAsync();
        //     return;
        // }
        //
        // var directories = Directory.GetDirectories(path);
        // var files = Directory.GetFiles(path);
        //
        // await writer.WriteAsync((directories.Length + files.Length).ToString());
        // await writer.FlushAsync();
        //
        // foreach (var file in files)
        // {
        //     Console.WriteLine(file);
        //     await writer.WriteAsync($" {file} false");
        //     await writer.FlushAsync();
        // }
        //
        // foreach (var directory in directories)
        // {
        //     await writer.WriteAsync($" {directory} true");
        //     await writer.FlushAsync();
        // }
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
        var tcpListener = new TcpListener(_ipAddress, _port);
        tcpListener.Start();
        while (true)
        {
            using var socket = tcpListener.AcceptSocket();
            Console.WriteLine(socket.AddressFamily);
            await using var newtworkStream = new NetworkStream(socket);
            using var streamReader = new StreamReader(newtworkStream);
            var strings = (streamReader.ReadLine())?.Split(' ');
            
            if (strings == null || strings.Length != 2)
            {
                continue;
            }

            var msgType = strings[0];
            if (msgType == "1")
            {
                Console.WriteLine("list");
                await Task.Run(() => List(newtworkStream, strings[1]));
            }

            if (msgType == "2")
            {
                Console.WriteLine("get");
                await Task.Run(() => Get(newtworkStream, strings[1]));
            }
        }
        
        tcpListener.Stop();
        
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

            switch (listOfArguments[0])
            {
                case "1":
                {
                    Console.WriteLine("list");
                    await Task.Run(() => List(stream, listOfArguments[1]));
                    break;
                }
                case "2":
                {
                    Console.WriteLine("get");
                    await Task.Run(() => Get(stream, listOfArguments[1]));
                    break;
                }
            }
        }

        listener.Stop();
    }
}