using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Websocket
{


    public class Server
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        public void Main()
        {
            Console.Title = "Server";
            SetupServer();
            Console.ReadLine();
            CloseAllSockets();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, SocketState.PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }
        private static void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            SocketState state = new SocketState();
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(state.buffer, 0, SocketState.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            serverSocket.BeginAccept(AcceptCallback, null);
            if(socket != null)
            {
                SendRequest(socket);
            }
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            SocketState state = new SocketState();
            Socket current = (Socket)AR.AsyncState;
            int received;
            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                current.Close();
                clientSockets.Remove(current);
                return;
            }


            byte[] recBuf = new byte[received];
            Array.Copy(state.buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Received Text: " + text);
            //   byte[] data = Encoding.ASCII.GetBytes(text);
            //current.Send(data);
            current.BeginReceive(state.buffer, 0, SocketState.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
        private static void ReceiveResponse()
        {
            SocketState state = new SocketState();
            int received = serverSocket.Receive(state.buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(state.buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text);
        }
        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            serverSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
        private static void SendRequest(Socket client)
        {
            while(client.Connected)
            {
                Console.Write("Send a request: ");
                string request = Console.ReadLine();
                SendString(request);

            }
            


        }
    }

}
