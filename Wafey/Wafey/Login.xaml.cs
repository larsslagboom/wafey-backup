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
using System.Windows.Shapes;
using System.Windows.Forms;

namespace Wafey
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        public int status { get; set; }
        private List<User> usersData;
        private DBHelper connection = new DBHelper();
        Hashing hasher = new Hashing();
        public User CurrentUser { get; private set; }
        public Login()
        {
            InitializeComponent();
            status = 0;
            PasswordBox.PasswordChar = '*';
            CurrentUser = new User();

        }

        private void test_Click(object sender, RoutedEventArgs e)
        {

            if(!string.IsNullOrWhiteSpace(LoginTextbox.Text) && !string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("http://145.44.233.180/selectuser.php?loginCheck&userLastName=");
                    sb.Append(LoginTextbox.Text);
                    sb.Append("&userEmail=");
                    sb.Append(LoginTextbox.Text);

                    usersData = connection.getData<User>(sb.ToString());
                    if (usersData.Count > 0 && hasher.VerifyPassword(PasswordBox.Password, usersData.First().UserPassword, usersData.First().UserSalt))
                    {
                        try
                        {
                            CurrentUser = CurrentUser.loadCurrentUser(usersData.First().UserID);
                            if(CurrentUser != null)
                            {
                                status = 1;
                                this.Close();
                            }
                            else
                            {
                                Errorbox.Text = "Sorry something went wrong";
                                LoginTextbox.Text = "";
                                PasswordBox.Password = "";
                            }
                        }
                        catch (Exception)
                        {
                            Errorbox.Text = "Sorry something went wrong";
                            LoginTextbox.Text = "";
                            PasswordBox.Password = "";
                        }
                    }
                    else
                    {
                        Errorbox.Text = "You entered the wrong credentials";
                        LoginTextbox.Text = "";
                        PasswordBox.Password = "";
                    }

                }
                catch(Exception)
                {
                    Errorbox.Text = "Sorry something went wrong";
                    LoginTextbox.Text = "";
                    PasswordBox.Password = "";
                }
            }
            else
            {
                LoginTextbox.Text = "";
                PasswordBox.Password = "";
            }

        }

        private void test2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
