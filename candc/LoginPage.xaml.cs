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
using CC.Providers;
using CC.Models;
using CC.Constants;
using System.Text.RegularExpressions;

namespace CC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public UserProvider userProvider { get; set; }

        private string mode;
        public string Mode
        {
            get { return mode; }
            set
            {
                mode = value;

                switch (mode)
                {
                    case "Login":
                        MainGrid.Visibility = Visibility.Hidden;
                        LoadingLabel.Visibility = Visibility.Visible;
                        break;
                    case "ChangePassRequired":
                        MainGrid.Visibility = Visibility.Hidden;
                        LoadingLabel.Visibility = Visibility.Hidden;
                        ChangePassGrid.Visibility = Visibility.Visible;
                        ErrorLabel.Content = string.Empty;
                        break;
                    case "WrongPass":
                        MainGrid.Visibility = Visibility.Visible;
                        LoadingLabel.Visibility = Visibility.Hidden;
                        ErrorLabel.Content = "Invalid Credentials";
                        break;
                    case "ChangePass":
                        ChangePassGrid.Visibility = Visibility.Hidden;
                        LoadingLabel.Visibility = Visibility.Visible;
                        break;
                    case "ChangePassFailed":
                        ChangePassGrid.Visibility = Visibility.Visible;
                        LoadingLabel.Visibility = Visibility.Hidden;
                        break;

                }

            }
        }

        public LoginPage()
        {
            InitializeComponent();

            userProvider = new UserProvider();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Mode = "Login";
            ErrorLabel.Content = string.Empty;
            userProvider.ValidateCredentials(UserNameTxt.Text.Trim(), PasswordTxt.Password.Trim());

            if (App.LoggedInUser != null)
            {
                if (App.LoggedInUser.RequirePasswordChange)
                {
                    Mode = "ChangePassRequired";
                }
                else
                    PrepareEnvironment();
            }
            else
            {
                Mode = "WrongPass";
            }
        }

        private void ChangePassButton_Click(object sender, RoutedEventArgs e)
        {
            if (PassTextbox.Text.Trim() != ConfPassTextbox.Text.Trim())
            {
                ErrorLabel.Content = "Password does not match in text boxes above";
                return;
            }

            if (!Regex.IsMatch(PassTextbox.Text.Trim(), UsersConsts.PasswordRegex))
            {
                ErrorLabel.Content = "Password must be between 6 to 25 characters and contain both letters and numbers";
                return;
            }

            // show animated wait until db read tasks are done 
            Mode = "ChangePass";

            // update password
            try
            {
                App.LoggedInUser.Password = PassTextbox.Text.Trim();
                App.LoggedInUser.RequirePasswordChange = false;
                var response = userProvider.UpdateUser(App.LoggedInUser, AuditEvents.PasswordChanged.ToString());

                if (!string.IsNullOrEmpty(response))
                {
                    ErrorLabel.Content = response;
                    Mode = "ChangePassFailed";
                }
                else
                    PrepareEnvironment();
            }
            catch (Exception ex)
            {
                var logNumber = ex.Data["logNumber"];
                MessageBox.Show($"{Messages.Exception} - logNumber={logNumber}");
            }
        }

        private void PrepareEnvironment()
        {  

            if (App.LoggedInUser.IsAdmin)
                NavigationService.Navigate(App.userMgmtPage);
            else
                NavigationService.Navigate(App.batchPage);

        }
    }
}
