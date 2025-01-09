using System.Windows;

namespace NetworkProgrammingApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //MainWindow clientWindow = new MainWindow();
            ServerWindow serverWindow = new ServerWindow();

            RestaurantWindow restaurantWindow = new RestaurantWindow();
            restaurantWindow.Show();
            serverWindow.Show();
        }
    }
}

/// </summary>
public partial class App : Application
    {
    }

