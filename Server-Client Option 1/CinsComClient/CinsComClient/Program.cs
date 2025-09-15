using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CinsComClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Connecting to server...");
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2588));
                Console.WriteLine("Connected to the server!");

                Thread receiveThread = new Thread(() => ReceiveData(clientSocket));
                receiveThread.Start();

                while (true)
                {
                    Console.Write("Enter message: ");
                    string message = Console.ReadLine();

                    byte[] buffer = Encoding.ASCII.GetBytes(message);
                    clientSocket.Send(buffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void ReceiveData(Socket clientSocket)
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    string message = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine($"Server: {message}");
                }
                catch
                {
                    Console.WriteLine("Disconnected from server.");
                    break;
                }
            }
        }
    }
}
