using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkProgrammingApp
{
    public partial class RestaurantWindow : Window
    {
        private const int RestaurantPort = 5002;

        public RestaurantWindow()
        {
            InitializeComponent();
            StartListeningForNotifications();
        }

        private async void StartListeningForNotifications()
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, RestaurantPort);
            listener.Start();
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleNotification(client));
            }
        }

        private async Task HandleNotification(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string notificationMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Dispatcher.Invoke(() => NotificationListBox.Items.Add(notificationMessage));

            client.Close();
        }
    }
}
