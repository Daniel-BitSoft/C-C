using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CC
{
    /// <summary>
    /// Interaction logic for UserManagementPage.xaml
    /// </summary>
    public partial class UserManagementPage : Page
    {
        public UserManagementPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ActiveUsersRadBtn.IsChecked = true;
            RefreshGridData(App.UserProvider.ActiveUsers);
        }

        private void ActiveUsersRadBtn_Checked(object sender, RoutedEventArgs e)
        {
            DisabledUserRadBtn.IsChecked = false;
            RefreshGridData(App.UserProvider.ActiveUsers);
        }

        private void DisabledUserRadBtn_Checked(object sender, RoutedEventArgs e)
        {
            ActiveUsersRadBtn.IsChecked = false;
            RefreshGridData(App.UserProvider.DisabledUsers);
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var users = App.UserProvider.GetUserByUsername(SearchTextBox.Text.Trim(), ActiveUsersRadBtn.IsChecked.HasValue && ActiveUsersRadBtn.IsChecked.Value);
                RefreshGridData(users);
            }
        }

        private void RefreshGridData(List<User> users)
        {
            UsersGrid.ItemsSource = users;
            UsersGrid.Items.Refresh();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            App.userPage.User = UsersGrid.SelectedItem as User;
            App.userPage.IsNew = false;
            NavigationService.Navigate(App.userPage);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            App.userPage.IsNew = true;
            NavigationService.Navigate(App.userPage);
        }
    }
}
