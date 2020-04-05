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
            User = null;
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

            DeleteButton.Visibility = Visibility.Hidden;
            ResetPasswordButton.Visibility = Visibility.Hidden;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validate())
                    return;

                if (IsNew)
                {
                    var response = App.UserProvider.CreateUser(new User
                    {
                        FirstName = FirstNameTextbox.Text.Trim(),
                        LastName = LastNameTextbox.Text.Trim(),
                        Username = EmailTextbox.Text.Trim(),
                        IsAdmin = AdminRadioBtn.IsChecked.HasValue && AdminRadioBtn.IsChecked.Value,
                        Password = TemporaryPassTextBox.Text.Trim()
                    }, AuditEvents.UserCreated.ToString());

                    if (!string.IsNullOrEmpty(response))
                    {
                        MessageBox.Show(response, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        App.UserProvider.UpdateUsersList().Wait();
                    }
                }
                else
                {
                    AuditEvents auditEvent = AuditEvents.UserUpdated;
                    if (User.IsDisabled != (DisabledCheckbox.IsChecked.HasValue && DisabledCheckbox.IsChecked.Value))
                    {
                        auditEvent = AuditEvents.UserAccessChanged;
                    }
                    if (User.IsAdmin != (AdminRadioBtn.IsChecked.HasValue && AdminRadioBtn.IsChecked.Value))
                    {
                        auditEvent = AuditEvents.AdminPrivilegeChanged;
                    }
                    if (TemporaryPassTextBox.Visibility == Visibility.Visible)
                    {
                        auditEvent = AuditEvents.PasswordReset;
                    }

                    User.FirstName = FirstNameTextbox.Text.Trim();
                    User.LastName = LastNameTextbox.Text.Trim();
                    User.Username = EmailTextbox.Text.Trim();
                    User.IsAdmin = AdminRadioBtn.IsChecked.HasValue && AdminRadioBtn.IsChecked.Value;
                    User.IsDisabled = DisabledCheckbox.IsChecked.HasValue && DisabledCheckbox.IsChecked.Value;

                    var response = App.UserProvider.UpdateUser(User, auditEvent.ToString());

                    if (!string.IsNullOrEmpty(response))
                    {
                        MessageBox.Show(response, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        App.UserProvider.UpdateUsersList().Wait();
                    }
                }

                App.UserProvider.UpdateUsersList().Wait();
                NavigationService.GoBack();
            }
            catch (Exception)
            {
                MessageBox.Show(Messages.Exception, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool Validate()
        {
            bool isvalid = true;
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(FirstNameTextbox.Text))
            {
                FNameLbl.BorderBrush = Brushes.Red;
                FNameLbl.BorderThickness = new Thickness(2);
                isvalid = false;
            }
            else
            {
                FNameLbl.BorderThickness = new Thickness(0);
            }

            if (string.IsNullOrWhiteSpace(LastNameTextbox.Text))
            {
                LNameLbl.BorderBrush = Brushes.Red;
                LNameLbl.BorderThickness = new Thickness(2);
                isvalid = false;
            }
            else
            {
                LNameLbl.BorderThickness = new Thickness(0);
            }

            if (IsNew && string.IsNullOrWhiteSpace(TemporaryPassTextBox.Text))
            {
                TempPassLabel.BorderBrush = Brushes.Red;
                TempPassLabel.BorderThickness = new Thickness(2);
                isvalid = false;
            }
            else
            {
                TempPassLabel.BorderThickness = new Thickness(0);
            }

            if (EmailTextbox.Text.Trim() != ConfirmEmailTextbox.Text.Trim() || string.IsNullOrWhiteSpace(EmailTextbox.Text))
            {
                EmailLbl.BorderBrush = ConfEmailLabel.BorderBrush = Brushes.Red;
                EmailLbl.BorderThickness = ConfEmailLabel.BorderThickness = new Thickness(2);

                errorMessages.Add("* Email does not match");
                isvalid = false;
            }
            else
            {
                EmailLbl.BorderThickness = ConfEmailLabel.BorderThickness = new Thickness(0);
            }

            if (!isvalid)
            {
                errorMessages.Add("* Please complete the form before saving");
                ErrorMessages.Text = string.Join("\r\n", errorMessages);
            }
            else
            {
                ErrorMessages.Text = string.Empty;
            }

            return isvalid;
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
                ConfirmEmailTextbox.Text = User.Username;
                AdminRadioBtn.IsChecked = User.IsAdmin;
                UserRadioBtn.IsChecked = !User.IsAdmin;
                LockCheckbox.IsChecked = User.IsLocked;
                DisabledCheckbox.IsChecked = User.IsDisabled;

                TemporaryPassTextBox.Visibility = Visibility.Hidden;
                TempPassLabel.Visibility = Visibility.Hidden;
            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will assign a new password to user and unlocks it. Are you sure you want to reset user's password? " +
                "\r\n\r\n ** You will need to click Save button after closing this message to store new password", "RESET", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TemporaryPassTextBox.Visibility = Visibility.Visible;
                TempPassLabel.Visibility = Visibility.Visible;
                TemporaryPassTextBox.Text = UsersConsts.DefaultTempPassword;
                DisabledCheckbox.IsChecked = false;
                IsNew = true;

                User.Password = UsersConsts.DefaultTempPassword;
                User.IsLocked = false;
                User.RequirePasswordChange = true;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
