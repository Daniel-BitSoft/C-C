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
            if (User.IsDisabled.HasValue && User.IsDisabled.Value)
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

                LockCheckbox.IsChecked = User.IsLocked.HasValue && User.IsLocked.Value;
                DisabledCheckbox.IsChecked = User.IsDisabled.HasValue && User.IsDisabled.Value;
            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will assign a new password to user and unlocks it. Are you sure you want to reset user's password?", "RESET", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                User.Password = !string.IsNullOrEmpty(TemporaryPassTextBox.Text) ? UsersConsts.DefaultTempPassword : TemporaryPassTextBox.Text.Trim();
                User.IsLocked = false;
                User.IsFirstLogin = true;

                App.UserProvider.UpdateUser(User);

                MessageBox.Show($"Successfully reset password. Temporary password is set to '{UsersConsts.DefaultTempPassword}'");
            }
        }
    }
}
