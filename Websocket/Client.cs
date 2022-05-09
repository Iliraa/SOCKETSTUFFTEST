using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Websocket
{
    public class Client
    {
        private static readonly Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public void Main()
        {
            Console.Title = "Client";
            ConnectToServer();
         //   RequestLoop();
            Exit();
        }

        private static void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    ClientSocket.Connect(IPAddress.Loopback, SocketState.PORT);
                  //  ClientSocket.Listen(0);
                    //ClientSocket.BeginAccept(AcceptCallback, null);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            while(Client.ClientSocket.Connected)
            {
                SendRequest();
 
            }

            Console.Clear();
            Console.WriteLine("Connected");
        }

        private static void RequestLoop()
        {
            while (true)
            {
                SendRequest();
                ReceiveResponse();
            }
        }
        private static void AcceptCallback(IAsyncResult AR)
        {
            SocketState state = new SocketState();
            Socket socket;

            try
            {
                socket = ClientSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            socket.BeginReceive(state.buffer, 0, SocketState.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            ClientSocket.BeginAccept(AcceptCallback, null);
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
            int received = ClientSocket.Receive(state.buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(state.buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text);
        }
        private static void Exit()
        {
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }
        private static void SendRequest()
        {
            Console.Write("Send a request: ");
            string request = Console.ReadLine();
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }
}
