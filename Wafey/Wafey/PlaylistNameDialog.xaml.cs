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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PlaylistNameDialog : Window
    {
        private List<Playlist> playlists = null;
        private bool overcharlimit = false;
        public PlaylistNameDialog(List<Playlist> playlists)
        {
            this.playlists = playlists;
            InitializeComponent();
            TextBoxPlaylistName.Focus();
        }

        private void ButtonNameCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxPlaylistName.Text))
            {
                Playlist testforname = null;
                try
                {
                    testforname = playlists.Find(p => p.GetName().Equals(TextBoxPlaylistName.Text));
                }
                catch (Exception)
                {

                }
                if (testforname == null) {
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("You already have a playlist with the same name");
                }
            } else
            {
                MessageBox.Show("Enter a name for the playlist");
            }
        }
        public string GetPlaylistName
        {
            get { return TextBoxPlaylistName.Text; }
            set { TextBoxPlaylistName.Text = value; }
        }

        private void ButtonNameCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBoxPlaylistName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(TextBoxPlaylistName.Text))
                {
                    Playlist testforname = null;
                    try
                    {
                        testforname = playlists.Find(p => p.GetName().Equals(TextBoxPlaylistName.Text));
                    }
                    catch (Exception)
                    {

                    }
                    if (overcharlimit)
                    {
                        MessageBox.Show("The name is longer than the 14 character limit");
                    }
                    else if (testforname == null)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("You already have a playlist with the same name");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a name for the playlist");
                }
            }
        }
        private void TextBoxPlaylistName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TextBoxPlaylistName.Text.Length > 14)
            {
                overcharlimit = true;
                ButtonNameCreate.IsEnabled = false;
            } else
            {
                overcharlimit = false;
                ButtonNameCreate.IsEnabled = true;
            }
        }
    }
}
