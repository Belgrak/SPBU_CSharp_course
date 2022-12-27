using System.Net;
using System.Net.Sockets;

namespace Client;

public class Client
{
    private readonly IPAddress _ipAddress;
    private readonly int _port;

    public Client(IPAddress ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    public async Task<(int, string[])> List(string path)
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

        var response = data.Split(' ');
        if (!int.TryParse(response[0], out int size))
        {
            throw new InvalidDataException();
        }

        if (size == -1)
        {
            throw new DirectoryNotFoundException();
        }

        return (size, response[Range.StartAt(1)]);
    }

    public async Task<(int, string)> Get(string path)
    {
        var client = new TcpClient();
        await client.ConnectAsync(_ipAddress, _port);

        await using var stream = client.GetStream();
        await using var writer = new StreamWriter(stream);

        await writer.WriteLineAsync($"2 {path}");
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

        return (size, answerList[1]);
    }
}