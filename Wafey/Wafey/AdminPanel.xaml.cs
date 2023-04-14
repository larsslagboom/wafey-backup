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
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        private DBHelper connection = new DBHelper();
        private VariableUI variableUI = new VariableUI();

        public int CurrentElementID { get; set; }

        public List<Song> songs = new List<Song>();

        private enum moduleTypes { Songs, Artists, Users}
        private moduleTypes module { get; set; }


        public int state { get; set; } = 1;

        public AdminPanel()
        {
            InitializeComponent();
            this.CurrentElementID = 0;

            this.module = moduleTypes.Songs;

            SelectedModule();

        }

        private void SelectedModule()
        {

            SearchElements.Name = "SearchElements" + module.ToString();

            if (module.Equals(moduleTypes.Songs))
            {
                songs = GetAllElements<Song>("http://145.44.233.180/playlist.php");
                ChangeTitleAdminPanel(songs.Count);
                LoadLeftSongPanel(songs, null);
            }
        }

        //Loads left side with buttons
        public void LoadLeftSongPanel(List<Song> songs, string search)
        {

            GridAllList.Children.Clear();
            GridAllList.RowDefinitions.Clear();
            GridAllList.ColumnDefinitions.Clear();
            int rowCount = 0;
            GridAllList.ShowGridLines = false;
            ColumnDefinition colDef1 = new ColumnDefinition();
            GridAllList.ColumnDefinitions.Add(colDef1);

            foreach (Song song in songs)
            {
                //Row
                RowDefinition SongListRow = new RowDefinition();
                SongListRow.Height = new GridLength(50);
                GridAllList.RowDefinitions.Add(SongListRow);
                //TextSongName/Author/length text
                variableUI.CreateTextBlock(this, GridAllList, song, rowCount, 1, VariableUI.TextblockType.AdminSongInfo, 60, 0, search);

                rowCount++;
            }

        }

        //load right side
        public void LoadRightPanel()
        {

            GridSpecific.Children.Clear();
            GridSpecific.RowDefinitions.Clear();
            GridSpecific.ColumnDefinitions.Clear();
            int rowCount = 0;
            GridSpecific.ShowGridLines = false;
            ColumnDefinition colDef1 = new ColumnDefinition();
            GridSpecific.ColumnDefinitions.Add(colDef1);

            if (module.Equals(moduleTypes.Songs))
            {
                Song songInfo = GetOneElement<Song>("http://145.44.233.180/song.php?songID=" + CurrentElementID);

                //change icon and name
                ChangeElementIconAndName(songInfo.SongName);

                variableUI.CreateLabel(this, GridSpecific, "Songname", 0, 1, VariableUI.LabelType.AdminPanelRightLabel, 60, 0);
                variableUI.CreateTextBox(this, GridSpecific, songInfo.SongName, "SongnameTextbox", 1, 1,  VariableUI.TextboxType.AdminPanelRightTextbox, 0, 0, true);
                variableUI.CreateLabel(this, GridSpecific, "Length", 2, 1, VariableUI.LabelType.AdminPanelRightLabel, 60, 0);

                string musicLength;
                if ((songInfo.Length % 60) < 10)
                {
                    musicLength = $"{songInfo.Length / 60}:0{songInfo.Length % 60}";
                }
                else
                {
                    musicLength = $"{songInfo.Length / 60}:{songInfo.Length % 60}";
                }

                variableUI.CreateTextBox(this, GridSpecific, musicLength, null, 3, 1, VariableUI.TextboxType.AdminPanelRightTextbox, 0, 0, false);

                variableUI.CreateLabel(this, GridSpecific, "Artist", 4, 1, VariableUI.LabelType.AdminPanelRightLabel, 60, 0);
                variableUI.CreateTextBox(this, GridSpecific, songInfo.Author, "SongArtistTextbox", 5, 1, VariableUI.TextboxType.AdminPanelRightTextbox, 0, 0, true);
            }
        }

        //change title function
        private void ChangeTitleAdminPanel(int count)
        {
            LabelComponentName.Content = $"Amount of {module.ToString()}: {count}";
        }

        //change element icon and name
        private void ChangeElementIconAndName(string nameElement)
        {
            ElementName.Text = nameElement;
            if (module.Equals(moduleTypes.Songs))
            {
                ElementIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Images/musicIcon.png"));
            }
        }

        //search event and function
        private void SearchElements_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox search = (TextBox) sender;

            searchResults(search);

        }
        public void searchResults(TextBox search)
        {
            if (module.Equals(moduleTypes.Songs))
            {

                List<Song> Searchedsongs = new List<Song>();

                if (search.Name.Replace("SearchElements", "").Equals("Songs"))
                {
                    songs = GetAllElements<Song>("http://145.44.233.180/playlist.php");
                    Searchedsongs = songs.Where(x => x.Author.ToLower().Contains(search.Text.ToLower()) || x.SongName.ToLower().Contains(search.Text.ToLower())).OrderBy(x => x.Author).ToList();
                    LoadLeftSongPanel(Searchedsongs, search.Text);
                }

                if (search.Text != "" && Searchedsongs.Count == 0)
                {
                    GridAllList.Children.Clear();
                    RowDefinition SongListRow = new RowDefinition();
                    SongListRow.Height = new GridLength(50);
                    GridAllList.RowDefinitions.Add(SongListRow);
                    TextBlock EmptyMessage = new TextBlock();
                    EmptyMessage.FontSize = 12;
                    EmptyMessage.HorizontalAlignment = HorizontalAlignment.Center;
                    EmptyMessage.Text = $"No result for '{search.Text}'";
                    EmptyMessage.Foreground = new SolidColorBrush(Colors.LightGray);
                    Grid.SetColumnSpan(EmptyMessage, 1);
                    Grid.SetRow(EmptyMessage, 0);
                    GridAllList.Children.Add(EmptyMessage);
                }
                
            }
        }
        //end search event and function

        //database functions
        public List<T> GetAllElements<T>(string link)
        {
            return connection.getData<T>(link);
        }
        
        private T GetOneElement<T>(string link)
        {
            try
            {
                return connection.getData<T>(link).First();
            }
            catch (Exception)
            {
                return default;
            }
        }

        private void DeleteElement_Click(object sender, RoutedEventArgs e)
        {
            string url = "";
            if (module.Equals(moduleTypes.Songs))
            {
                url = "http://145.44.233.180/song.php?deleteSong&songID=" + CurrentElementID;
            }

            connection.DeleteElement(url);
            ChangeTitleAdminPanel(songs.Count);
            Song removedSong = songs.Where(s => s.SongID.Equals(CurrentElementID)).ToList().First();
            ChangeElementIconAndName($"{removedSong.SongName} has been removed");
            GridSpecific.Children.Clear();
            GridSpecific.RowDefinitions.Clear();
            GridSpecific.ColumnDefinitions.Clear();
            hideButtonsRightPanel();

            if (SearchElements.Text != null)
            {
                searchResults(SearchElements);
            }
            
        }

        private void ChangeElement_Click(object sender, RoutedEventArgs e)
        {

            TextBox textboxName = new TextBox();
            TextBox textboxArtist = new TextBox();

            string url = "";
            if (module.Equals(moduleTypes.Songs))
            {
                foreach (UIElement child in GridSpecific.Children)
                {
                    if (child is TextBox)
                    {

                        TextBox textbox = child as TextBox;

                        if (textbox.Name.Equals("SongnameTextbox"))
                        {
                            textboxName = textbox;
                        }

                        if (textbox.Name.Equals("SongArtistTextbox"))
                        {
                            textboxArtist = textbox;
                        }
                    }
                }

                if (textboxName.Text != null && textboxName.Text != "" && textboxArtist.Text != null && textboxName.Text != "")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("http://145.44.233.180/song.php?changeSong&songID=" + CurrentElementID);
                    sb.Append("&songName=" + textboxName.Text);
                    sb.Append("&songAuthor=" + textboxArtist.Text);
                    url = sb.ToString();

                    if (!connection.ChangeElement(url))
                    {
                        Song songInfo = GetOneElement<Song>("http://145.44.233.180/song.php?songID=" + CurrentElementID);

                        if (songInfo == null)
                        {
                            ChangeTitleAdminPanel(songs.Count);
                            ChangeElementIconAndName("Song has not been found");
                            GridSpecific.Children.Clear();
                            GridSpecific.RowDefinitions.Clear();
                            GridSpecific.ColumnDefinitions.Clear();
                            hideButtonsRightPanel();
                        }

                    }
                    else
                    {
                        LoadLeftSongPanel(songs = GetAllElements<Song>("http://145.44.233.180/playlist.php"), null);
                        ChangeElementIconAndName(textboxName.Text);
                    }

                    if (SearchElements.Text != null)
                    {
                        searchResults(SearchElements);
                    }

                }
            }

            

        }

        public void hideButtonsRightPanel()
        {
            DeleteElement.Visibility = Visibility.Hidden;
            DeleteElementLabel.Visibility = Visibility.Hidden;
            ChangeElement.Visibility = Visibility.Hidden;
            ChangeElementLabel.Visibility = Visibility.Hidden;
        }

        private void DeleteElement_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteElement.Content = FindResource("DeleteSelect");
        }

        private void DeleteElement_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteElement.Content = FindResource("Delete");
        }

        private void ChangeElement_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeElement.Content = FindResource("EditSelect");
        }

        private void ChangeElement_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeElement.Content = FindResource("Edit");
        }
    }
}
