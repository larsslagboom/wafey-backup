using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Payment_confirm.xaml
    /// </summary>
    public partial class Payment_confirm : Window
    {

        public int confirmation { get; set; }
        private string methodPayment { get; set; }

        public Payment_confirm(User currentUser)
        {
            InitializeComponent();
            this.confirmation = 0;
            UserFirstname.Content = currentUser.UserFirstName;
            UserEmail.Content = currentUser.UserEmail;
            Username.Content = currentUser.UserLastName;

            AlertImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Checkmark.png"));

        }

        private void Confirm_payment_button_Click(object sender, RoutedEventArgs e)
        {
            MethodsPanel.Visibility = Visibility.Hidden;
            ConfirmPanel.Visibility = Visibility.Hidden;
            AlertPanel.Visibility = Visibility.Visible;

            AlertImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Checkmark.png"));
            AlertTitle.Content = "Payment successful";
            AlertMessage.Text = "Thank you for subscribing to the most awesome music streaming platform out there. You will not regret this decission, you're a new human. Head out and listen to that awsome music without ads.";

            confirmation = 1;

        }

        private void Cancel_payment_button_Click(object sender, RoutedEventArgs e)
        {
            MethodsPanel.Visibility = Visibility.Hidden;
            ConfirmPanel.Visibility = Visibility.Hidden;
            AlertPanel.Visibility = Visibility.Visible;

            AlertImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/NotCheckmark.png"));
            AlertTitle.Content = "Payment failed";
            AlertMessage.Text = "Something went wrong, please try again later.";

        }

        private void Button_method_Click(object sender, RoutedEventArgs e)
        {

            Button method = (Button)sender;

            this.methodPayment = method.Name.Replace("Button_", "");

            userMethod.Content = methodPayment;

            MethodsPanel.Visibility = Visibility.Hidden;
            ConfirmPanel.Visibility = Visibility.Visible;

        }

        private void Back_payment_button_Click(object sender, RoutedEventArgs e)
        {
            MethodsPanel.Visibility = Visibility.Visible;
            ConfirmPanel.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
