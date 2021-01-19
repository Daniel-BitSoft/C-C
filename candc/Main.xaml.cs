using System.Windows;

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

            this.Title = "C&C";

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

            if (App.LoggedInUser != null && App.LoggedInUser.IsAdmin)
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
            App.ccPageType = Constants.CCType.C;

            frame.Navigate(new CCPage());
        }

        private void NegMenuButton_Click(object sender, RoutedEventArgs e)
        {
            App.ccPageType = Constants.CCType.N;
            frame.Navigate(new CCPage());
        }

        private void PosMenuButton_Click(object sender, RoutedEventArgs e)
        {
            App.ccPageType = Constants.CCType.P;
            frame.Navigate(new CCPage());
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ViewArrays_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(new CCPage());
        }

        private void ViewCC_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(new ViewCCPage());
        }
    }
}
