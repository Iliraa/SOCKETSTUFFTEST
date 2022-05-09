using System.Net.Sockets;
using System.Net;
using System.Text;
using Websocket;



Console.WriteLine("1.server");
Console.WriteLine("2.cleint");
int a = Int32.Parse(Console.ReadLine()); 

if(a == 1)
{
    Server server = new Server();

    server.Main();
}
else if (a == 2)
{
    Client client = new Client();
    client.Main();
}