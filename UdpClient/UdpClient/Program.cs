using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UdpClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1. Create a socket
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // 2. Define server endpoint
                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1995);

                while (true)
                {
                    // 3. Send data to server
                    Console.Write("Client: ");
                    string message = Console.ReadLine();
                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                    clientSocket.SendTo(messageBytes, serverEndpoint);

                    // 4. Receive response from server
                    byte[] buffer = new byte[1024];
                    EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    int receivedBytes = clientSocket.ReceiveFrom(buffer, ref clientEndPoint);
                    string serverMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine("Server: " + serverMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}

