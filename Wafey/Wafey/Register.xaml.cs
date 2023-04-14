using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using MySql.Data.MySqlClient.Memcached;

namespace Wafey
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {

        public int status { get; set; }
        private bool name, email, username, pass, confirmpass = false;
        bool secondPass = false;
        private string password { get; set; }
        DBHelper connection = new DBHelper();
        Hashing hasher = new Hashing();
        HashSalt passwordOptions = new HashSalt();
        private List<User> usersData;
        private User CurrentUser { get; set; }

        public Register()
        {
            InitializeComponent();
            checkFields();
            this.status = 0;
            usersData = connection.getData<User>("http://145.44.233.180/selectuser.php?registerDupCheck");
            CurrentUser = new User();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            //checks for fields
            if (checkFields())
            {

                passwordOptions = hasher.GenerateSaltedHash(64, PasswordBox.Password);

                int checkUserID = connection.DBconnectie(EmailTextbox.Text, NameTextbox.Text, UsernameTextbox.Text, passwordOptions.Hash, passwordOptions.Salt);

                if (checkUserID != 0)
                {
                    CurrentUser = CurrentUser.loadCurrentUser(checkUserID);
                    if (CurrentUser != null)
                    {
                        status = 1;
                        this.Close();
                    }
                    else
                    {
                        Errorbox.Text = "Sorry something went wrong";
                    }
                }
                else
                {
                    Errorbox.Text = "Sorry something went wrong, try again later";
                }

            }

        }

        private void Check_for_empty(object sender, TextChangedEventArgs e)
        {
            TextBox textboxs = (TextBox)sender;

            if (textboxs.Equals(NameTextbox))
            {
                if (string.IsNullOrWhiteSpace(textboxs.Text))
                {
                    name = false;
                    NameLabel.Foreground = Brushes.Red;
                    Errorbox.Text = "This field cannot be empty";
                }
                else if (textboxs.Text.Contains(" "))
                {
                    name = false;
                    NameLabel.Foreground = Brushes.Red;
                    Errorbox.Text = "This field cannot contain space";
                }
                else
                {
                    name = true;
                    NameLabel.Foreground = Brushes.Black;
                    Errorbox.Text = null;
                }
            }

            if (textboxs.Equals(UsernameTextbox))
            {

                User userUsernames = usersData.Find(u => u.UserLastName.Equals(textboxs.Text));

                if (string.IsNullOrWhiteSpace(textboxs.Text))
                {
                    username = false;
                    UsernameLabel.Foreground = Brushes.Red;
                    Errorbox.Text = "This field cannot be empty";
                }
                else if (textboxs.Text.Contains(" "))
                {
                    username = false;
                    UsernameLabel.Foreground = Brushes.Red;
                    Errorbox.Text = "This field cannot contain space";
                }else if (userUsernames != null)
                {
                    username = false;
                    UsernameLabel.Foreground = Brushes.Red;
                    Errorbox.Text = "This username is already taken";
                }
                else
                {
                    username = true;
                    UsernameLabel.Foreground = Brushes.Black;
                    Errorbox.Text = null;
                }
            }

            checkFields();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void PasswordChangedHandler(Object sender, RoutedEventArgs args)
        {

            PasswordBox PasswordBox1 = (PasswordBox)sender;

            string password = PasswordBox1.Password;

            bool passChecks = password.Length >= 7
                              && password.Where(char.IsUpper).Count() >= 1
                              && password.Where(char.IsDigit).Count() >= 1
                              && !string.IsNullOrWhiteSpace(password)
                              && !password.Contains(" ");

            bool passSame = PasswordBox.Password.Equals(ConfirmPasswordBox.Password);

            if (passChecks && passSame)
            {
                PasswordLabel.Foreground = Brushes.Black;
                ConfirmPasswordLabel.Foreground = Brushes.Black;
                pass = true;
                confirmpass = true;
                Errorbox.Text = null;
            }
            else if (!passChecks && passSame)
            {
                PasswordLabel.Foreground = Brushes.Red;
                ConfirmPasswordLabel.Foreground = Brushes.Black;
                pass = false;
                confirmpass = false;
                Errorbox.Text =
                    "Make sure your password has more than 7 characters, has at least 1 capital letter, has at least 1 digit and doesn't contain a space!";
            }
            else if (passChecks && !passSame)
            {
                PasswordLabel.Foreground = Brushes.Black;
                ConfirmPasswordLabel.Foreground = Brushes.Red;
                pass = false;
                confirmpass = false;
                Errorbox.Text = "The passwords don't match!";
            }
            else
            {

                PasswordLabel.Foreground = Brushes.Red;
                pass = false;
                confirmpass = false;
                Errorbox.Text = "Make sure your password has more than 7 characters, has at least 1 capital letter, has at least 1 digit and doesn't contain a space!";
                if (PasswordBox1.Equals(ConfirmPasswordBox) && secondPass)
                {
                    ConfirmPasswordLabel.Foreground = Brushes.Red;
                }
                else
                {
                    secondPass = true;
                }

            }

            checkFields();

        }

    private void email_ChangedHandler(object sender, TextChangedEventArgs e)
    {
            TextBox EmailTextbox = (TextBox)sender;

            User userUsernames = usersData.Find(u => u.UserEmail.Equals(EmailTextbox.Text));

            Regex mRegxExpression;
            if (EmailTextbox.Text.Trim() != string.Empty)
            {
                mRegxExpression = new Regex(@"^([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$");

                if (!mRegxExpression.IsMatch(EmailTextbox.Text.Trim()))
                {
                    EmailLabel.Foreground = Brushes.Red;
                    email = false;
                    Errorbox.Text = "Please enter a valid email adress";
                }
                else if (userUsernames != null)
                {
                    username = false;
                    EmailLabel.Foreground = Brushes.Red;
                    Errorbox.Text = "This email is already taken";
                }
                else
                {
                    EmailLabel.Foreground = Brushes.Black;
                    email = true;
                    Errorbox.Text = null;
                }
            }

            checkFields();
        }

        public bool checkFields()
        {

            if (name && email && username && pass && confirmpass)
            {
                RegisterButton.IsEnabled = true;
            }
            else
            {
                RegisterButton.IsEnabled = false;
                return false;
            }
            return true;
        }

        public void SetFields(String name, String email, String username, String pass, String confpass)
        {
            NameTextbox.Text = name;
            EmailTextbox.Text = email;
            UsernameTextbox.Text = username;
            PasswordBox.Password = pass;
            ConfirmPasswordBox.Password = confpass;

            
        }

    }

}
