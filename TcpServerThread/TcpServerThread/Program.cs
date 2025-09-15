using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpServerThread
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Server soketi oluşturuluyor
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind işlemi
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 1995);
            serverSocket.Bind(endPoint);

            // Listen işlemi
            serverSocket.Listen(10);
            Console.WriteLine("Server is listening on port 1995...");

            // Client bağlantısını kabul et
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("Client connected!");

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
                        Console.WriteLine($"Client: {message}");
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Connection closed by client.");
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
            serverSocket.Close();

            /*
            TcpListener server = new TcpListener(IPAddress.Any, 1995);
            server.Start();
            Console.WriteLine("Server started. Waiting for connection...");

            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client connected!");

            NetworkStream stream = client.GetStream();

            // Mesaj alımı için bir thread
            Thread receiveThread = new Thread(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Client: {message}");
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
