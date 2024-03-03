using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

class Client
{
    public static async Task GetMessage(TcpClient client)
    {
        var stream = client.GetStream();
        List<byte> bytes = new List<byte>();

        while (true)
        {
            int bytes_read = 0;

            while ((bytes_read = stream.ReadByte()) != '\0')
            {
                bytes.Add((byte)bytes_read);
            }

            string str = Encoding.UTF8.GetString(bytes.ToArray());
            User user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(str);
            await Console.Out.WriteLineAsync($"{user.Name}: {user.Message}\n");

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


        TcpClient tcp_client = new TcpClient();
        await tcp_client.ConnectAsync(IPAddress.Parse(ip), port);
        await Console.Out.WriteAsync("Connected\n");


        var stream = tcp_client.GetStream();

        Task.Run(async () => await GetMessage(tcp_client));


        User user = new User();

        await Console.Out.WriteAsync("\nenter your name: ");
        user.Name = Console.ReadLine();
        await Console.Out.WriteLineAsync();

        while (true)
        {
            user.Message = Console.ReadLine();

            string str = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            str += '\0';


            byte[] bytes = Encoding.UTF8.GetBytes(str);

            await stream.WriteAsync(bytes);
        }
    }
}