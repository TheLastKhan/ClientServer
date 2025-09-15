using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UdpServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1. Create a socket
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // 2. Bind the socket to a specific port
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 1995);
                serverSocket.Bind(serverEndPoint);
                Console.WriteLine("UDP Server is running on port 1995");

                while (true)
                { 
                    // 3. Receive data from client
                    byte[] buffer = new byte[1024];
                    EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    int receivedBytes = serverSocket.ReceiveFrom(buffer, ref clientEndPoint);
                    string clientMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine("Client: " + clientMessage);

                    // 4. Send response to client
                    Console.Write("Server: ");
                    string response = Console.ReadLine();
                    byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                    serverSocket.SendTo(responseBytes, clientEndPoint);
                }
            }
            catch(Exception ex) 
            { 
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
