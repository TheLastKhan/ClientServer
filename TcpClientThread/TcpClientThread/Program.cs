using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpClientThread
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Client soketi oluşturuluyor
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Server'a bağlan
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1995);
            clientSocket.Connect(serverEndPoint);
            Console.WriteLine("Connected to server!");

            // Mesaj alımı için bir thread
            Thread receiveThread = new Thread(() =>
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
                    catch (SocketException)
                    {
                        Console.WriteLine("Connection closed by server.");
                        break;
                    }
                }
            });

            // Mesaj gönderimi için bir thread
            Thread sendThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        string message = Console.ReadLine();
                        byte[] buffer = Encoding.ASCII.GetBytes(message);
                        clientSocket.Send(buffer);
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Connection closed. Cannot send message.");
                        break;
                    }
                }
            });

            // Thread'leri başlat
            receiveThread.Start();
            sendThread.Start();

            // Thread'lerin bitmesini bekleyin
            receiveThread.Join();
            sendThread.Join();

            // Kaynakları serbest bırak
            clientSocket.Close();

            /*
            TcpClient client = new TcpClient("127.0.0.1", 1995);
            Console.WriteLine("Connected to server!");

            NetworkStream stream = client.GetStream();

            // Mesaj alımı için bir thread
            Thread receiveThread = new Thread(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Server: {message}");
                }
            });

            // Mesaj gönderimi için bir thread
            Thread sendThread = new Thread(() =>
            {
                while (true)
                {
                    string message = Console.ReadLine();
                    byte[] buffer = Encoding.ASCII.GetBytes(message);
                    stream.Write(buffer, 0, buffer.Length);
                }
            });

            // Thread'leri başlat
            receiveThread.Start();
            sendThread.Start();
            */
        }
    }
}
