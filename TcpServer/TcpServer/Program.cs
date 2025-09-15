using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TcpServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1. Create a socket
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 2. Bind the socket to a specific port
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 1995);
                serverSocket.Bind(serverEndPoint);
                Console.WriteLine("Server is running on port 1995...");

                // 3. Listen for incoming connections
                serverSocket.Listen(3);
                Console.WriteLine("Waiting for client connection...");

                // 4. Accept a connection
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("Client connected!");

                while (true)
                {
                    // 5. Receive data from client
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    string clientMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes); // ?
                    Console.WriteLine("Client: " + clientMessage);

                    if (clientMessage.ToLower() == "bye")
                    {
                        Console.WriteLine("Closing connection...");
                        clientSocket.Close();
                        break;  // Exit the loop to close the connection
                    }

                    // 6. Send response to client
                    Console.Write("Server: ");
                    string response = Console.ReadLine();

                    if (response.ToLower() == "bye")
                    {
                        Console.WriteLine("Closing connection...");
                        serverSocket.Close();
                        break;  // Exit the loop to close the connection
                    }

                    byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                    clientSocket.Send(responseBytes);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}

