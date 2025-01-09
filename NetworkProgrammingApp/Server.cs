using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkProgrammingApp
{
    class Server
    {
        private const int Port = 5000;
        private static Dictionary<string, string> orders = new Dictionary<string, string>();

        static async Task Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClient(client));
            }
        }

        private static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string order = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            string[] parts = order.Split(':');
            string orderId = parts[0];
            string orderDetails = parts[1];

            orders[orderId] = orderDetails;
            Console.WriteLine($"Received Order: {orderId} - {orderDetails}");

            // Simulate server choosing an order to confirm
            Console.WriteLine("Enter Order ID to confirm:");
            string confirmOrderId = Console.ReadLine();

            if (orders.ContainsKey(confirmOrderId))
            {
                string confirmationMessage = $"Order with OrderID = {confirmOrderId} has been confirmed.";
                byte[] confirmationData = Encoding.UTF8.GetBytes(confirmationMessage);
                await stream.WriteAsync(confirmationData, 0, confirmationData.Length);
                Console.WriteLine(confirmationMessage);
            }

            client.Close();
        }
    }
}
