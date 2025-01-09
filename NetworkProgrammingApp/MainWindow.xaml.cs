using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkProgrammingApp
{
    public partial class MainWindow : Window
    {
        private const string ServerIp = "127.0.0.1";
        private const int ServerPort = 5000;
        private const int ClientPort = 5001;
        private const string PlaceholderFoodList = "Enter Food List";
        private const string PlaceholderAddress = "Enter Address";
        private int currentOrderId = 1;

        public MainWindow()
        {
            InitializeComponent();
            FoodListTextBox.Text = PlaceholderFoodList;
            FoodListTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            AddressTextBox.Text = PlaceholderAddress;
            AddressTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            StartListeningForConfirmations();
        }

        private void SendOrderButton_Click(object sender, RoutedEventArgs e)
        {
            string foodList = FoodListTextBox.Text;
            string address = AddressTextBox.Text;
            if (!string.IsNullOrEmpty(foodList) && foodList != PlaceholderFoodList &&
                !string.IsNullOrEmpty(address) && address != PlaceholderAddress)
            {
                string orderId = GenerateOrderId();
                string orderWithId = $"{orderId}:{foodList}:{address}";
                SendOrderToServer(orderWithId);
                FoodListTextBox.Clear();
                FoodListTextBox.Text = PlaceholderFoodList;
                FoodListTextBox.Foreground = System.Windows.Media.Brushes.Gray;
                AddressTextBox.Clear();
                AddressTextBox.Text = PlaceholderAddress;
                AddressTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
            else
            {
                MessageBox.Show("Please enter food list and address.");
            }
        }

        private string GenerateOrderId()
        {
            return currentOrderId++.ToString("D5");
        }

        private void SendOrderToServer(string order)
        {
            try
            {
                using (TcpClient client = new TcpClient(ServerIp, ServerPort))
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(order);
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

        private async void StartListeningForConfirmations()
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, ClientPort);
            listener.Start();
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleConfirmation(client));
            }
        }

        private async Task HandleConfirmation(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string confirmationMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Dispatcher.Invoke(() => ConfirmationListBox.Items.Add(confirmationMessage));

            client.Close();
        }
        //Just for UI hehe
        private void FoodListTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FoodListTextBox.Text == PlaceholderFoodList)
            {
                FoodListTextBox.Text = string.Empty;
                FoodListTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void FoodListTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FoodListTextBox.Text))
            {
                FoodListTextBox.Text = PlaceholderFoodList;
                FoodListTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void AddressTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (AddressTextBox.Text == PlaceholderAddress)
            {
                AddressTextBox.Text = string.Empty;
                AddressTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                AddressTextBox.Text = PlaceholderAddress;
                AddressTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
