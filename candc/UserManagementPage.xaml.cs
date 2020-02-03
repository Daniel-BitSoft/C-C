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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void RefreshGridData(List<CC.User> users)
        {
            UsersGrid.ItemsSource = App.mapper.Map<List<Models.User>>(users);
            UsersGrid.Items.Refresh();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(App.userPage);
        }

        private void Page_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
