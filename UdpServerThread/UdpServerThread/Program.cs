using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UdpServerThread
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 1995;
            UdpClient server = new UdpClient(port); // UDP soketi oluşturulur
            Console.WriteLine($"UDP Server is listening on port {port}...");
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Thread receiveThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        // Mesaj alınıyor
                        byte[] receivedData = server.Receive(ref clientEndPoint);
                        string receivedMessage = Encoding.ASCII.GetString(receivedData);
                        Console.WriteLine($"Client: {receivedMessage}");
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
                        // Server'dan mesaj gönderiliyor
                        Console.Write("Server: ");
                        string message = Console.ReadLine();
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        server.Send(data, data.Length, clientEndPoint);
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

            /*
            int port = 1995;
            UdpClient server = new UdpClient(port); // UDP soketi oluşturulur
            Console.WriteLine($"UDP Server is listening on port {port}...");
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Thread receiveThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        // Mesaj alınıyor
                        byte[] receivedData = server.Receive(ref clientEndPoint);
                        string receivedMessage = Encoding.ASCII.GetString(receivedData);
                        Console.WriteLine($"Client: {receivedMessage}");
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
                        // Server'dan mesaj gönderiliyor
                        Console.Write("Server: ");
                        string message = Console.ReadLine();
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        server.Send(data, data.Length, clientEndPoint);
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
