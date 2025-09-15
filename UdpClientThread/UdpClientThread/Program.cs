using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UdpClientThread
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string serverIP = "127.0.0.1"; // Server'ın IP adresi
            int serverPort = 1995;         // Server'ın portu
            int clientPort = 2000;         // Client'ın yerel portu

            // Soket oluştur ve yerel IP:Port'a bağla
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, clientPort);
            clientSocket.Bind(clientEndPoint); // Client soketini belirli bir porta bağla

            Console.WriteLine($"Client is bound to port {clientPort}");

            Thread receiveThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        EndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

                        // Server'dan mesaj al
                        int receivedBytes = clientSocket.ReceiveFrom(buffer, ref serverEndPoint);
                        string receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

                        Console.WriteLine($"Server [{serverEndPoint}]: {receivedMessage}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error (Receive): {ex.Message}");
                    }
                }
            });

            Thread sendThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Console.Write("Client: ");
                        string message = Console.ReadLine();
                        byte[] buffer = Encoding.ASCII.GetBytes(message);

                        // Mesajı server'a gönder
                        clientSocket.SendTo(buffer, new IPEndPoint(IPAddress.Parse(serverIP), serverPort));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error (Send): {ex.Message}");
                    }
                }
            });

            receiveThread.Start();
            sendThread.Start();
            receiveThread.Join();
            sendThread.Join();

            /*
            string serverIP = "127.0.0.1"; // Server'ın IP adresi
            int serverPort = 1995; // Server'ın portu
            UdpClient client = new UdpClient();

            Thread receiveThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] receivedData = client.Receive(ref serverEndPoint);
                        string receivedMessage = Encoding.ASCII.GetString(receivedData);
                        Console.WriteLine($"Server: {receivedMessage}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            });

            Thread sendThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Console.Write("Client: ");
                        string message = Console.ReadLine();
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        client.Send(data, data.Length, serverIP, serverPort);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            });

            receiveThread.Start();
            sendThread.Start();
            receiveThread.Join();
            sendThread.Join();
            */
        }
    }
}
