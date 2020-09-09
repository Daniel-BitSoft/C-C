using CC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<User> users;

        public UserManagementPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            users = App.UserProvider.GetAllUsers();
            ActiveUsersRadBtn.IsChecked = true;
            RefreshGridData(true);
        }

        private void ActiveUsersRadBtn_Checked(object sender, RoutedEventArgs e)
        {
            DisabledUserRadBtn.IsChecked = false;
            RefreshGridData(true);
        }

        private void DisabledUserRadBtn_Checked(object sender, RoutedEventArgs e)
        {
            ActiveUsersRadBtn.IsChecked = false;
            RefreshGridData(false);
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && users != null && users.Any())
            {
                IEnumerable<User> result;
                if (Convert.ToBoolean(ActiveUsersRadBtn.IsChecked.Value))
                {
                    result = users.Where(a => !a.IsDisabled &&
                        (a.FirstName.Contains(SearchTextBox.Text.Trim()) ||
                         a.LastName.Contains(SearchTextBox.Text.Trim()) ||
                         a.Username.Contains(SearchTextBox.Text.Trim())));
                }
                else
                {
                    result = users.Where(a => a.IsDisabled &&
                       (a.FirstName.Contains(SearchTextBox.Text.Trim()) ||
                        a.LastName.Contains(SearchTextBox.Text.Trim()) ||
                        a.Username.Contains(SearchTextBox.Text.Trim())));
                }

                UsersGrid.ItemsSource = result;
                UsersGrid.Items.Refresh();
            }
        }

        private void RefreshGridData(bool active)
        {
            UsersGrid.ItemsSource = users.Where(a => a.IsDisabled != active).ToList();
            UsersGrid.Items.Refresh();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditUser();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            App.userPage.IsNew = true;
            NavigationService.Navigate(App.userPage);
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditUser();
        }

        private void EditUser()
        {
            if (UsersGrid.SelectedItem != null)
            {
                App.userPage.User = UsersGrid.SelectedItem as User;
                App.userPage.IsNew = false;
                NavigationService.Navigate(App.userPage);
            }
        }

    }
}
