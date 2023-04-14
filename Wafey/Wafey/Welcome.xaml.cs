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

namespace Wafey
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {

        public int state { get; set; }
        public User CurrentUser { get; private set; }
        DBHelper connection = new DBHelper();
        public int Guestnr {get;set;}

        public Welcome()
        {
            InitializeComponent();

            state = 1;
            CurrentUser = new User();
            /*this.connection.Open();
            this.connection.Close();*/

        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {

            Login login = new Login();
            login.ShowDialog();

            if (login.status == 1)
            {
                CurrentUser = login.CurrentUser;
                if (CurrentUser.Admin.Equals(1)) {
                    this.Visibility = Visibility.Hidden;
                    AdminPanel admin = new AdminPanel();
                    admin.ShowDialog();

                    if (admin.state.Equals(2))
                    {
                        this.Close();
                    }
                    else
                    {
                        this.Visibility = Visibility.Visible;
                    }


                } else {
                    this.state = 2;
                    this.Close();
                }
            }

        }

        public void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            Guestnr = r.Next(0, 99999999);
            int checkUserID = connection.DBconnectie("Guest" + Guestnr + "@wafey.com", "GuestUser", "Guest" + Guestnr, "-", "-");

            Console.WriteLine(checkUserID);

            if (checkUserID != 0)
            {
                CurrentUser = CurrentUser.loadCurrentUser(checkUserID);
                if (CurrentUser != null)
                {
                    this.state = 2;
                    this.Close();
                }
                else
                {
                    Errorbox.Content = "Sorry something went wrong";
                }
            }
            else
            {
                Errorbox.Content = "Sorry something went wrong, try again later";
                
            }

            Console.WriteLine("executed");
            this.Close();
        }

       

        public void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.ShowDialog();
            
            

            if (register.status == 1)
            {
                this.state = 2;
                this.Close();
            }
        }

       
    }
}
