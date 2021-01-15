using CC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using CC.Providers;
using System.Text.RegularExpressions;

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

            TemporaryPassTextBox.Visibility = TempPassLabel.Visibility = Visibility.Visible;
            TemporaryPassTextBox.Text = UsersConsts.DefaultTempPassword;

            DeleteButton.Visibility = Visibility.Hidden;
            ResetPasswordButton.Visibility = Visibility.Hidden;
            LockCheckbox.Visibility = Visibility.Hidden;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validate())
                    return;

                var users = App.UserProvider.GetAllUsers();

                if (IsNew)
                {
                    if (users.Any(a => string.Equals(a.Username, EmailTextbox.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        if (MessageBox.Show($"Username '{EmailTextbox.Text.Trim()}' is already taken", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }

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
                }
                else
                {
                    if (Convert.ToBoolean(DisabledCheckbox.IsChecked))
                    {
                        if (MessageBox.Show("You are about to disable this user which will revoke the permision to use this software. Would you like to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    if (User.IsAdmin && (Convert.ToBoolean(DisabledCheckbox.IsChecked) || Convert.ToBoolean(UserRadioBtn.IsChecked)))
                    {
                        if (!users.Any(a => !a.IsDisabled && a.IsAdmin && a.Username != User.Username))
                        {
                            MessageBox.Show("This user is the only admin and cannot be disabled. At least one admin is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

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
                }

                NavigationService.Navigate(App.userMgmtPage);
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(UserPage) + nameof(SaveButton_Click), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                MessageBox.Show($"{ Messages.Exception} - log: {logNumber}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool Validate()
        {
            bool isvalid = true;
            bool incompleteForm = false;
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(FirstNameTextbox.Text))
            {
                FNameLbl.BorderBrush = Brushes.Red;
                FNameLbl.BorderThickness = new Thickness(2);
                isvalid = false;
                incompleteForm = true;
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
                incompleteForm = true;
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
                incompleteForm = true;
            }
            else
            {
                TempPassLabel.BorderThickness = new Thickness(0);
            }

            if (TemporaryPassTextBox.Visibility == Visibility.Visible && !Regex.IsMatch(TemporaryPassTextBox.Text.Trim(), UsersConsts.PasswordRegex))
            {
                errorMessages.Add("* Password must be between 6 to 25 characters and contain both letters and numbers");
                isvalid = false;
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
                if (incompleteForm)
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
                    NavigationService.Navigate(App.userMgmtPage);
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
            else if (User != null)
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

                DisabledCheckbox.Visibility = Visibility.Visible;
                LockCheckbox.Visibility = Visibility.Visible;
                ResetPasswordButton.Visibility = Visibility.Visible;

                if (User.IsDisabled)
                    DeleteButton.Visibility = Visibility.Visible;
                else
                    DeleteButton.Visibility = Visibility.Hidden;
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
                IsNew = false;

                User.Password = UsersConsts.DefaultTempPassword;
                User.IsLocked = false;
                User.RequirePasswordChange = true;
                User.LockCounter = 0;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
