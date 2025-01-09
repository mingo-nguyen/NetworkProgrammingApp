using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkProgrammingApp
{
    public partial class ServerWindow : Window
    {
        private const int Port = 5000;
        private const int ClientPort = 5001;
        private const int RestaurantPort = 5002; 
        private ObservableCollection<Order> orders = new ObservableCollection<Order>();

        public ServerWindow()
        {
            InitializeComponent();
            OrdersDataGrid.ItemsSource = orders;
            StartServer();
        }

        private async void StartServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClient(client));
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string order = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            string[] parts = order.Split(':');
            string orderId = parts[0];
            string foodList = parts[1];
            string address = parts[2];

            Dispatcher.Invoke(() => orders.Add(new Order { OrderId = orderId, FoodList = foodList, Address = address }));

            client.Close();
        }

        private void ConfirmOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                string confirmationMessage = $"Order with OrderID = {selectedOrder.OrderId} has been confirmed.";
                SendConfirmationToClient(confirmationMessage);
                SendNotificationToRestaurant("New Order Placed");
                orders.Remove(selectedOrder);
            }
            else
            {
                MessageBox.Show("Please select an order to confirm.");
            }
        }

        private void SendConfirmationToClient(string message)
        {
            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", ClientPort))
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private void SendNotificationToRestaurant(string message)
        {
            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", RestaurantPort))
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public string FoodList { get; set; }
        public string Address { get; set; }
    }
}

