using CC.Constants;
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
    /// Interaction logic for CreateUserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        public bool IsNew { get; set; }
        public User User { get; set; }

        public UserPage()
        {
            InitializeComponent();
        }

        private void PrepareNewCustomer()
        {
            FirstNameTextbox.Text =
            LastNameTextbox.Text =
            EmailTextbox.Text =
            ConfirmEmailTextbox.Text =
            TemporaryPassTextBox.Text =
            string.Empty;

            UserRadioBtn.IsChecked = true;
            AdminRadioBtn.IsChecked = false;

            DisabledCheckbox.IsChecked = false;
            DisabledCheckbox.Visibility = Visibility.Hidden;

            TemporaryPassTextBox.Visibility = Visibility.Visible;
            TemporaryPassTextBox.Text = UsersConsts.DefaultTempPassword;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsNew)
            {
                App.UserProvider.CreateUser(new User
                {
                    FirstName = FirstNameTextbox.Text.Trim(),
                    LastName = LastNameTextbox.Text.Trim(),
                    Username = EmailTextbox.Text.Trim(),
                    IsAdmin = AdminRadioBtn.IsChecked.HasValue && AdminRadioBtn.IsChecked.Value,
                    Password = TemporaryPassTextBox.Text.Trim()
                });
            }
            else
            {
                User.FirstName = FirstNameTextbox.Text.Trim();
                User.LastName = LastNameTextbox.Text.Trim();
                User.Username = EmailTextbox.Text.Trim();
                User.IsAdmin = AdminRadioBtn.IsChecked.HasValue && AdminRadioBtn.IsChecked.Value;
                User.IsDisabled = DisabledCheckbox.IsChecked.HasValue && DisabledCheckbox.IsChecked.Value;

                App.UserProvider.UpdateUser(User);
            }

            NavigationService.GoBack();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.IsDisabled)
            {
                if (MessageBox.Show("This will DELETE the user forever. Are you sure you want to continue?", "DELETE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    App.UserProvider.DeleteUser(User);
                }
            }
            else
            {
                MessageBox.Show("User is not disabled. Only disabled users can be deleted", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsNew)
                PrepareNewCustomer();
            else
            {
                FirstNameTextbox.Text = User.FirstName;
                LastNameTextbox.Text = User.LastName;
                EmailTextbox.Text = User.Username;
                AdminRadioBtn.IsChecked = User.IsAdmin;
                UserRadioBtn.IsChecked = !User.IsAdmin;
                LockCheckbox.IsChecked = User.IsLocked;
                DisabledCheckbox.IsChecked = User.IsDisabled;

                ConfirmEmailTextbox.Visibility = Visibility.Hidden;
                ConfEmailLabel.Visibility = Visibility.Hidden;
                TemporaryPassTextBox.Visibility = Visibility.Hidden;
                TempPassLabel.Visibility = Visibility.Hidden;
            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will assign a new password to user and unlocks it. Are you sure you want to reset user's password?", "RESET", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TemporaryPassTextBox.Visibility = Visibility.Visible;
                TempPassLabel.Visibility = Visibility.Visible;
                TemporaryPassTextBox.Text = UsersConsts.DefaultTempPassword;
                DisabledCheckbox.IsChecked = false;

                User.Password = UsersConsts.DefaultTempPassword;
                User.IsLocked = false;
                User.RequirePasswordChange = true;
            }
        }
    }
}
