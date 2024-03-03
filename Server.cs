using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


class User
{
    public string Name { get; set; }

    public string Message { get; set; }
}

class Server
{
    public static List<TcpClient> Clients = new List<TcpClient>();


    public static async Task ProcessClient(TcpClient client)
    {
        var stream = client.GetStream();
        List<byte> bytes = new List<byte>();

        while (true)
        {
            int bytes_read = 0;

            while (true)
            {
                bytes_read = stream.ReadByte();

                bytes.Add((byte)bytes_read);

                if (bytes_read == '\0') break;
            }

            for (int i = 0; i < Clients.Count; i++)
            {
                if (client != Clients[i] && Clients[i].Connected)
                {
                    var send_message_stream = Clients[i].GetStream();

                    await send_message_stream.WriteAsync(bytes.ToArray());
                }
            }

            bytes.Clear();
        }
    }

    public static async Task Main(string[] args)
    {
        string ip;
        int port;

        await Console.Out.WriteAsync("ip: ");
        ip = Console.ReadLine();

        await Console.Out.WriteAsync("port: ");
        port = Convert.ToInt32(Console.ReadLine());


        TcpListener tcp_listener = new TcpListener(IPAddress.Parse(ip), port);
        tcp_listener.Start();

        Console.WriteLine("Server started...\n");


        while (true)
        {
            TcpClient tcp_client = await tcp_listener.AcceptTcpClientAsync();

            await Console.Out.WriteLineAsync("\nClient started...\n");

            Clients.Add(tcp_client);

            Task.Run(async() => await ProcessClient(tcp_client));
        }
    }
}