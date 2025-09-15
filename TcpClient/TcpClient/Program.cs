using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TcpClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1. Create a socket
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 2. Connect to the server
                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1995);
                clientSocket.Connect(serverEndpoint);
                Console.WriteLine("Connected to the server!");

                while (true) 
                {
                    // 3. Send data to server
                    Console.Write("Client: ");
                    string message = Console.ReadLine();

                    if (message.ToLower() == "bye")
                    {
                        Console.WriteLine("Closing connection...");
                        break; // Exit the loop to close the connection
                    }

                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                    clientSocket.Send(messageBytes);

                    // 4. Receive response from server
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    string serverMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine("Server: " + serverMessage);

                    if (serverMessage.ToLower() == "bye")
                    {
                        Console.WriteLine("Closing connection...");
                        clientSocket.Close();
                        break;  // Exit the loop to close the connection
                    }

                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
