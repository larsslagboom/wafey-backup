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
    /// Interaction logic for RemovePlaylistConfirmationWindow.xaml
    /// </summary>
    public partial class RemovePlaylistConfirmationWindow : Window
    {
        public RemovePlaylistConfirmationWindow(string playlistname)
        {
            InitializeComponent();
            TextblockConfirmation.Text = $"Are you sure you want to delete playlist '{playlistname}'?";
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
