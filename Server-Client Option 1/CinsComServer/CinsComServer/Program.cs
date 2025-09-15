using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CinsComServer
{
    internal class Program
    {
        static List<ClientInfo> clients = new List<ClientInfo>();
        static List<string> news = new List<string>(); // Last-minute news (max 3)
        static void Main(string[] args)
        {
            Console.WriteLine("CinsCOM Server is starting...");
            TcpListener listener = new TcpListener(IPAddress.Any, 2588);
            listener.Start();
            Console.WriteLine("Server is running on port 2588...");

            // Thread to update weather and exchange rates periodically
            Thread updateThread = new Thread(UpdateData);
            updateThread.Start();

            while (true)
            {
                Socket clientSocket = listener.AcceptSocket();
                Console.WriteLine($"Client connected: {clientSocket.RemoteEndPoint}");

                // Create a new client handler
                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();
            }
        }

        static void UpdateData()
        {
            while (true)
            {
                // Fetch weather and exchange rates here (simplified example)
                string weather = "Temperature: 15°C, Humidity: 80%";
                string exchangeRates = "USD: 27.00, EUR: 30.00";

                Console.WriteLine($"Weather updated: {weather}");
                Console.WriteLine($"Exchange rates updated: {exchangeRates}");

                // Wait 60 seconds before the next update
                Thread.Sleep(60000);
            }
        }

        static void HandleClient(Socket clientSocket)
        {
            ClientInfo clientInfo = new ClientInfo
            {
                Socket = clientSocket,
                Name = clientSocket.RemoteEndPoint.ToString()
            };
            clients.Add(clientInfo);

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    string message = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

                    Console.WriteLine($"Received: {message}");
                    ProcessMessage(clientInfo, message);
                }
                catch
                {
                    Console.WriteLine($"{clientInfo.Name} disconnected.");
                    clients.Remove(clientInfo);
                    clientSocket.Close();
                    break;
                }
            }
        }

        static void ProcessMessage(ClientInfo client, string message)
        {
            if (message.StartsWith("NEWS:"))
            {
                string newNews = message.Substring(5);
                if (news.Count == 3) news.RemoveAt(0); // Keep max 3 news
                news.Add(newNews);

                BroadcastToAll($"NEWS_UPDATE:{string.Join(" | ", news)}");
            }
            else if (message.StartsWith("SEND:"))
            {
                string[] parts = message.Substring(5).Split('|');
                string recipientName = parts[0];
                string msg = parts[1];

                ClientInfo recipient = clients.Find(c => c.Name == recipientName);
                if (recipient != null)
                {
                    SendMessage(recipient, $"FROM:{client.Name}|{msg}");
                }
                else
                {
                    SendMessage(client, "ERROR:Recipient not found.");
                }
            }
            else if (message == "REQUEST_DATA")
            {
                string data = $"WEATHER:Temperature: 15°C, Humidity: 80%\n" +
                              $"EXCHANGE:USD: 27.00, EUR: 30.00\n" +
                              $"NEWS:{string.Join(" | ", news)}";
                SendMessage(client, data);
            }
            else
            {
                SendMessage(client, $"ECHO:{message}");
            }
        }

        static void BroadcastToAll(string message)
        {
            foreach (var client in clients)
            {
                SendMessage(client, message);
            }
        }

        static void SendMessage(ClientInfo client, string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            client.Socket.Send(buffer);
        }
        class ClientInfo
        {
            public Socket Socket { get; set; }
            public string Name { get; set; }
        }
    }
}
