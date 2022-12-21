using System.Net;
using System.Net.Sockets;

namespace SimpleFTP;

public class Client
{
    private readonly IPAddress _ipAddress;
    private readonly int _port;
    
    public Client(IPAddress ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }
    
    public async void List(string path)
    {
        var client = new TcpClient();
        await client.ConnectAsync(_ipAddress, _port);

        await using var stream = client.GetStream();
        await using var writer = new StreamWriter(stream);
        
        await writer.WriteLineAsync($"1 {path}");
        await writer.FlushAsync();
        
        
        using var reader = new StreamReader(stream);
        var data = await reader.ReadLineAsync();
        if (data == null)
        {
            throw new InvalidDataException();
        }

        var answerList = data.Split(' ');
        if (!int.TryParse(answerList[0], out int size))
        {
            throw new InvalidDataException();
        }

        if (size == -1)
        {
            throw new DirectoryNotFoundException();
        }



    }
    
    public async void Writer(NetworkStream stream)
    {
        Task.Run(async () =>
        {
            var writer = new StreamWriter(stream) { AutoFlush = true };
            while (true)
            {
                Console.WriteLine(">");
                var data = Console.ReadLine();
                await writer.WriteAsync(data + "\n");
            }
        });
    }
}