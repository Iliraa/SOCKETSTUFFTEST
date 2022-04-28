using System.Net.Sockets;
using System.Net;
using System.Text;
using Websocket;




Console.WriteLine("1.server");
Console.WriteLine("2.cleint");
int a = Int32.Parse(Console.ReadLine()); 

if(a == 1)
{
    Server();
}
else if (a == 2)
{
    Client();
}
static void Client()
{
    AsynchronousClient client = new AsynchronousClient();
    client.Test();
}
static void Server()
{
    AsynchronousSocketListener server = new AsynchronousSocketListener();
    server.Test();
}

