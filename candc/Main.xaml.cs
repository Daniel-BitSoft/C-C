using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CC
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        LoginPage loginPage;

        public Main()
        {
            InitializeComponent();

            // initial pages
            loginPage = new LoginPage();

            // Navigate to first page which is login
            frame.Navigate(loginPage);
        }

        private void frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (App.LoggedInUser != null && string.IsNullOrEmpty(userLabel.Text))
            {
                userLabel.Text = $"User: {App.LoggedInUser.FirstName}";
                MainMenu.Visibility = Visibility.Visible;
            }

            if (App.LoggedInUser != null && App.LoggedInUser.IsAdmin.HasValue && App.LoggedInUser.IsAdmin.Value)
            {
                AdminMenu.Visibility = Visibility.Visible;
            }
            else
            {
                AdminMenu.Visibility = Visibility.Hidden;
            }
        }

        private void UsersMenuButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(App.userMgmtPage);
        }

        private void CalibMenuButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(App.cCPage);
        }

        private void NegMenuButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(App.cCPage);
        }

        private void PosMenuButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(App.cCPage);
        }

        private void AntigenMenuButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(App.AntigenPage);
        }

        private void ArrayMenuButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(App.arrayPage);
        }

        private void AssignBatchButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(App.batchPage);
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(App.userPage);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Task> loadDataTasks = new List<Task>();

            loadDataTasks.Add(App.UserProvider.UpdateUsersList());


            Task.WaitAll(loadDataTasks.ToArray());
        }
    }
}
