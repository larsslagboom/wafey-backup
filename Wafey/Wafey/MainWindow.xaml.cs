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
using System.Media;
using Wafey.Properties;
using System.IO;
using System.Threading;
using System.Net;
using System.Runtime.Remoting;
using System.Windows.Threading;
using Newtonsoft.Json.Schema;

namespace Wafey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //instances
        private MusicPlayer musicplayer = new MusicPlayer();
        private VariableUI variableUI = new VariableUI();
        public Welcome welcome = new Welcome();
        private DBHelper connection = new DBHelper();

        //state variables
        public bool playing = false;
        public bool IsPlaylist = false;
        public string sortby;
        public enum PanelType { UserPanel,HomePanel,PlaylistsPanel,SearchPanel}
        PanelType selectedPanel = PanelType.HomePanel;
        //windows username 
        private string userName = "";
        // Bool for detecting first time clicking play button.
        private bool firstTimePlaying = true;
        //playlist & song list
        public List<Song> songs = new List<Song>();
        public List<Playlist> playlists = new List<Playlist>();
        //selected playlist & song
        public Song selectedSong = null;
        public Playlist selectedPlaylist = null;
        public Playlist favorite = new Playlist("Favorite", 1);
        //logged in user
        public User currentUser { get; set; }


        private Thread playingSong;
        private bool changeTime = true;

        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DispatcherTimer disTimer = new DispatcherTimer();
        
        //Volume icon
        public bool volumeMuted;
        public double PreviousVolume;
        public object PreviousImage;

        public MainWindow()
        {
            InitializeComponent();
            IsEnabled = false;
            welcome.ShowDialog();
            this.userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string[] echteUserName = userName.Split('\\');
            this.userName = echteUserName[echteUserName.Length - 1];
            songs = GetAllSongs();
            List<Song> alphabeticsongs = songs.OrderBy(x => x.SongName).ToList();
            songs = alphabeticsongs;
            ButtonHome.Content = FindResource("HomeActive");
            Console.WriteLine(songs[0].Genre.ToString());
            if (welcome.state == 2 && welcome.CurrentUser != null)
            {
                currentUser = welcome.CurrentUser;
                IsEnabled = true;
                loadPlaylistsFromDB();
                LoadSongs();
                LoadPlaylists();
                UsernameLabel.Content = currentUser.UserLastName;
                LoadUserInfo();
                Advertisement ad = new Advertisement(this);
            }
            else
            {
                this.Close();
            }
            
        }
        
        /// <summary>
        /// Loads all playlists with with buttons
        /// </summary>
        public void LoadPlaylists()
        {
            GridPlaylists.Children.Clear();
            GridPlaylists.RowDefinitions.Clear();
            GridPlaylists.ColumnDefinitions.Clear();
            int rowCount = 0;
            int columnCount = 0;
            GridPlaylists.ShowGridLines = false;
            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            ColumnDefinition colDef3 = new ColumnDefinition();
            GridPlaylists.ColumnDefinitions.Add(colDef1);
            GridPlaylists.ColumnDefinitions.Add(colDef2);
            GridPlaylists.ColumnDefinitions.Add(colDef3);
            foreach (Playlist playlist in playlists)
            {
                //Row
                if (columnCount == 0)
                {
                    RowDefinition PlaylistRow = new RowDefinition();
                    PlaylistRow.Height = new GridLength(230);
                    GridPlaylists.RowDefinitions.Add(PlaylistRow);
                }

                if (playlist.GetFavorite().Equals(1))
                {
                    variableUI.CreatePlaylistButton(this, GridPlaylists, playlist, rowCount, columnCount, VariableUI.ButtonType.SelectPlaylistFavorite);
                }
                else
                {
                    //ButtonSelectPlaylist
                    variableUI.CreatePlaylistButton(this, GridPlaylists, playlist, rowCount, columnCount, VariableUI.ButtonType.SelectPlaylist);
                }
                //Counters
                if (columnCount == 2)
                {
                    rowCount++;
                }
                if (columnCount == 2)
                {
                    columnCount = 0;
                } else
                {
                    columnCount++;
                }
            }

            //Adds playlist create button
            if (columnCount == 0)
            {
                RowDefinition PlaylistRow = new RowDefinition();
                PlaylistRow.Height = new GridLength(230);
                GridPlaylists.RowDefinitions.Add(PlaylistRow);
            }
            variableUI.CreatePlaylistButton(this, GridPlaylists, null, rowCount, columnCount, VariableUI.ButtonType.AddPlaylist);
        }
        /// <summary>
        /// Loads the current selected playlist with songs and buttons
        /// </summary>
        public void LoadPlaylist()
        {
            //selectPlaylist();
            if (selectedPlaylist != null)
            {
                RowDefinition SongListRowEmpty = new RowDefinition();
                Label LabelEmptyPlaylist = new Label();
                ScrollbarPlaylist.Visibility = Visibility.Visible;
                LabelPlaylistName.Visibility = Visibility.Visible;
                LabelPlaylistName.Content = "Playlist: " + selectedPlaylist.GetName();
                GridPlaylist.Children.Clear();
                GridPlaylist.RowDefinitions.Clear();
                GridPlaylist.ColumnDefinitions.Clear();
                int rowCount = 0;
                GridPlaylist.ShowGridLines = false;
                ColumnDefinition colDef1 = new ColumnDefinition();
                GridPlaylist.ColumnDefinitions.Add(colDef1);
                if (selectedPlaylist.GetSonglist().Count != 0)
                {
                    foreach (Song song in selectedPlaylist.GetSonglist())
                    {
                        //Row
                        RowDefinition SongListRow = new RowDefinition();
                        SongListRow.Height = new GridLength(50);
                        GridPlaylist.RowDefinitions.Add(SongListRow);
                        //Playbutton
                        variableUI.CreateButton(this, GridPlaylist, rowCount, song, VariableUI.ButtonType.Play, 30, 30, HorizontalAlignment.Left, VerticalAlignment.Center, 0, true, null);
                        //SongInfoText
                        variableUI.CreateTextBlock(this, GridPlaylist, song, rowCount, VariableUI.TextblockType.AllSongInfo, 33, 0);
                        //Removebutton
                        variableUI.CreateButton(this, GridPlaylist, rowCount, song, VariableUI.ButtonType.RemoveFromPlaylist, 30, 30, HorizontalAlignment.Right, VerticalAlignment.Center, 0, true, null);

                        rowCount++;
                    }
                }
                else
                {
                    SongListRowEmpty.Height = new GridLength(50);
                    GridPlaylist.RowDefinitions.Add(SongListRowEmpty);
                    LabelEmptyPlaylist.Foreground = new SolidColorBrush(Colors.LightGray);
                    LabelEmptyPlaylist.Content = "This playlist is empty";
                    Grid.SetRow(LabelEmptyPlaylist, rowCount);
                    GridPlaylist.Children.Add(LabelEmptyPlaylist);
                }
            }
            else
            {
                GridPlaylist.Children.Clear();
                GridPlaylist.RowDefinitions.Clear();
                GridPlaylist.ColumnDefinitions.Clear();
                ScrollbarPlaylist.Visibility = Visibility.Hidden;
                LabelPlaylistName.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// Loads all songs with buttons
        /// </summary>
        public void LoadSongs()
        {
            GridContent.Children.Clear();
            GridContent.RowDefinitions.Clear();
            GridContent.ColumnDefinitions.Clear();
            int rowCount = 0;
            GridContent.ShowGridLines = false;
            ColumnDefinition colDef1 = new ColumnDefinition();
            GridContent.ColumnDefinitions.Add(colDef1);

            foreach (Song song in songs)
            {
                //Row
                RowDefinition SongListRow = new RowDefinition();
                SongListRow.Height = new GridLength(50);
                GridContent.RowDefinitions.Add(SongListRow);
                //Playbutton
                variableUI.CreateButton(this, GridContent, rowCount, song, VariableUI.ButtonType.Play, 48, 48, HorizontalAlignment.Left, VerticalAlignment.Center, 0, false, null);
                if (selectedPlaylist != null)
                {

                    if (selectedPlaylist.GetFavorite().Equals(0))
                    {
                        //Addbutton
                        variableUI.CreateButton(this, GridContent, rowCount, song, VariableUI.ButtonType.AddToPlaylist, 48, 48, HorizontalAlignment.Right, VerticalAlignment.Center, 0, false, null);
                        //AddFavoritebutton
                        variableUI.CreateButton(this, GridContent, rowCount, song, VariableUI.ButtonType.AddToFavorite, 48, 48, HorizontalAlignment.Right, VerticalAlignment.Center, 60, false, null);
                    }
                    else
                    {
                        //AddFavoritebutton
                        variableUI.CreateButton(this, GridContent, rowCount, song, VariableUI.ButtonType.AddToFavorite, 48, 48, HorizontalAlignment.Right, VerticalAlignment.Center, 0, false, null);
                    }
                }
                else
                {
                    //Addbutton
                    variableUI.CreateButton(this, GridContent, rowCount, song, VariableUI.ButtonType.AddToPlaylist, 48, 48, HorizontalAlignment.Right, VerticalAlignment.Center, 0, false, null);
                    //AddFavoritebutton
                    variableUI.CreateButton(this, GridContent, rowCount, song, VariableUI.ButtonType.AddToFavorite, 48, 48, HorizontalAlignment.Right, VerticalAlignment.Center, 60, false, null);
                }
                //TextSongName/Author text
                variableUI.CreateTextBlock(this, GridContent, song, rowCount, VariableUI.TextblockType.NameAndAuthor, 60, 0);
                //TextSongLength
                variableUI.CreateTextBlock(this, GridContent, song, rowCount, VariableUI.TextblockType.SongLength, 0, 120);

                rowCount++;
            }
        }
        /// <summary>
        /// Merges the two playlist queries into one playlist and puts it in "playlists"
        /// </summary>
        public void loadPlaylistsFromDB()
        {
            List<Playlist> filledlist = GetFilledPlaylists(currentUser.UserID);
            List<Playlist> visuallist = GetVisualPlaylists(currentUser.UserID);
            favorite.SetSonglist(GetFavPlaylists(currentUser.UserID));
            List<Playlist> actuallist = new List<Playlist>();
            bool inlist = new bool();

            if (!favorite.GetSonglist().Count.Equals(0))
            {
                actuallist.Add(favorite);
            }

            foreach (Playlist plv in visuallist)
            {
                foreach (Playlist plf in filledlist)
                {
                    if (plv.GetPlaylistID().Equals(plf.GetPlaylistID()))
                    {
                        actuallist.Add(plf);
                        inlist = true;
                        break;
                    }
                    else
                    {
                        inlist = false;
                    }

                }
                if (!inlist)
                {
                    actuallist.Add(plv);
                }
            }
            playlists = actuallist;
        }

        /// <summary>
        /// Streams a song from the given URL
        /// </summary>
        /// <param name="url"></param>
        public void playSong(string url)
        {
            {
                timeSlider.Value = 0;
                musicplayer.url = url;
                if (!firstTimePlaying)
                {
                    playingSong.Abort();
                }
                playingSong = new Thread(new ThreadStart(musicplayer.streamAudio));
                playingSong.Start();

                
                if (firstTimePlaying)
                {
                    disTimer.Tick += updateTimeSlider;
                    disTimer.Interval = TimeSpan.FromSeconds(1);
                    disTimer.Start();
                }
                

                firstTimePlaying = false;

                if (dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Stop();
                }
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(ButtonSkipForward_Clicker);
                while (musicplayer.length == 0 || musicplayer.currentTime == 0)
                {
                    Thread.Sleep(100);
                }
                dispatcherTimer.Interval = TimeSpan.FromSeconds((musicplayer.length - musicplayer.currentTime));
                dispatcherTimer.Start();

            }
        }

        #region Database Methods
        private List<Song> GetAllSongs()
        {
            return connection.getData<Song>("http://145.44.233.180/playlist.php");
        }

        private List<Song> GetFavPlaylists(int UserID)
        {

            List<Song> fav = connection.getData<Song>("http://145.44.233.180/favorite.php?getList&UserID=" + UserID);

            if (fav.Count > 0)
            {
                return fav;
            }

            return new List<Song>();
        }

        private List<Playlist> GetVisualPlaylists(int UserID)
        {
            return connection.getData<Playlist>("http://145.44.233.180/GetUserPlaylists.php?UserID=" + UserID); ;
        }

        private List<Playlist> GetFilledPlaylists(int UserID)
        {
            return connection.getData<Playlist>("http://145.44.233.180/GetUserPlaylists.php?allUserPlaylist&UserID=" + UserID);
        }

        /// <summary>
        /// Removes the playist with PlaylistID
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        public bool removePlaylist(int playlistID)
        {
            String URL = "http://145.44.233.180/playlist.php?removePlaylist=" + playlistID;

            string result = new WebClient().DownloadString(URL);

            if (result.Equals("success"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the Playlist with playlistname to UserID
        /// </summary>
        /// <param name="playlistName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool addPlaylist(string playlistName, int userID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://145.44.233.180/playlist.php?addPlaylist&playlistName=");
            sb.Append(playlistName);
            sb.Append("&userID=");
            sb.Append(userID);
            String URL = sb.ToString();

            string result = new WebClient().DownloadString(URL);

            if (result.Equals("success"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes SongID from PlaylistID
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="songID"></param>
        /// <returns></returns>
        public bool removePlaylistSong(int playlistID,int songID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://145.44.233.180/playlist.php?removePlaylistSong&playlistID=");
            sb.Append(playlistID);
            sb.Append("&songID=");
            sb.Append(songID);
            String URL = sb.ToString();

            string result = new WebClient().DownloadString(URL);


            if (result.Equals("success"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the SongID to the PlaylistID
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="songID"></param>
        /// <returns></returns>
        public bool addPlaylistSong(int playlistID, int songID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://145.44.233.180/playlist.php?addPlaylistSong&playlistID=");
            sb.Append(playlistID);
            sb.Append("&songID=");
            sb.Append(songID);
            String URL = sb.ToString();

            string result = new WebClient().DownloadString(URL);

            if (result.Equals("success"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the SongID to favourites of UserID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="songID"></param>
        /// <returns></returns>
        public bool addFavoriteSong(int userID, int songID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://145.44.233.180/favorite.php?addSong&UserID=");
            sb.Append(userID);
            sb.Append("&SongID=");
            sb.Append(songID);
            String URL = sb.ToString();

            string result = new WebClient().DownloadString(URL);

            if (result.Equals("success"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the SongID from the favourites of UserID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="songID"></param>
        /// <returns></returns>
        public bool removeFavoriteSong(int userID, int songID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://145.44.233.180/favorite.php?delSong&UserID=");
            sb.Append(userID);
            sb.Append("&SongID=");
            sb.Append(songID);
            String URL = sb.ToString();

            string result = new WebClient().DownloadString(URL);


            if (result.Equals("success"))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region ClickEvents
        //####################################################################################################################################################################################
        private void ButtonSkipForward_Clicker(object sender, EventArgs e)
        {
            skipForward();
        }
        public void skipForward()
        {
            dispatcherTimer.Stop();
            dispatcherTimer = new DispatcherTimer();
            musicplayer.currentTime = 0;
            timeSlider.Value = 0;
            try
            {
                if (selectedSong != null && !IsPlaylist)
                {
                    if (songs[songs.IndexOf(selectedSong) + 1] != null)
                    {
                        selectedSong = songs[songs.IndexOf(selectedSong) + 1];
                        playSong(selectedSong.SongLocation);
                        playing = true;
                        ButtonPlayPause.Content = FindResource("Pause");
                        LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                        LoadSongs();
                        LoadPlaylist();
                    }
                }
                else if (selectedSong != null && IsPlaylist)
                {
                    try
                    {
                        List<Song> songlist = selectedPlaylist.GetSonglist();
                        if (songlist[songlist.IndexOf(selectedSong) + 1] != null)
                        {
                            selectedSong = songlist[songlist.IndexOf(selectedSong) + 1];
                            playSong(selectedSong.SongLocation);
                            playing = true;
                            ButtonPlayPause.Content = FindResource("Pause");
                            LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                            LoadSongs();
                            LoadPlaylist();
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("No list selected");
                    }
                }
                else
                {
                    Console.WriteLine("No list selected");
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                if (IsPlaylist)
                {
                    selectedSong = selectedPlaylist.GetSonglist()[0];
                }
                else
                {
                    selectedSong = songs[0];
                }
                playSong(selectedSong.SongLocation);
                playing = true;
                ButtonPlayPause.Content = FindResource("Pause");
                LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                LoadPlaylist();
                LoadSongs();
            }

        }

        private void ButtonPlayPause_Click(object sender, RoutedEventArgs e)
        {
            playing = !playing;

            if (firstTimePlaying)
            {
                musicplayer.url = "http://145.44.233.180/MUSIC/Leek_spin.mp3";

                playSong(musicplayer.url);

                ButtonPlayPause.Content = FindResource("PauseSelect");

                firstTimePlaying = false;


            }
            else if (playing)
            {

                ButtonPlayPause.Content = FindResource("PauseSelect");

                musicplayer.play();
                dispatcherTimer.Stop();
                dispatcherTimer = new DispatcherTimer();

                dispatcherTimer.Tick += new EventHandler(ButtonSkipForward_Clicker);
                dispatcherTimer.Interval = TimeSpan.FromSeconds((musicplayer.length - musicplayer.currentTime));
                Console.WriteLine(musicplayer.length - musicplayer.currentTime);
                dispatcherTimer.Start();

            }
            else if (!playing)
            {
                ButtonPlayPause.Content = FindResource("PlaySelect");
                musicplayer.pause();
                dispatcherTimer.Stop();
                dispatcherTimer = new DispatcherTimer();

            }
        }


        private void ButtonSkipForward_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            dispatcherTimer = new DispatcherTimer();
            musicplayer.currentTime = 0;
            timeSlider.Value = 0;
            try
            {
                if (selectedSong != null && !IsPlaylist)
                {
                    if (songs[songs.IndexOf(selectedSong) + 1] != null)
                    {
                        selectedSong = songs[songs.IndexOf(selectedSong) + 1];
                        playSong(selectedSong.SongLocation);
                        playing = true;
                        ButtonPlayPause.Content = FindResource("Pause");
                        LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                        LoadSongs();
                        LoadPlaylist();
                    }
                }
                else if (selectedSong != null && IsPlaylist)
                {
                    try
                    {
                        List<Song> songlist = selectedPlaylist.GetSonglist();
                        if (songlist[songlist.IndexOf(selectedSong) + 1] != null)
                        {
                            selectedSong = songlist[songlist.IndexOf(selectedSong) + 1];
                            playSong(selectedSong.SongLocation);
                            playing = true;
                            ButtonPlayPause.Content = FindResource("Pause");
                            LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                            LoadSongs();
                            LoadPlaylist();
                        }
                    } catch(NullReferenceException)
                    {
                        Console.WriteLine("No list selected");
                    }
                }
                else
                {
                    Console.WriteLine("No list selected");
                }

            } catch(ArgumentOutOfRangeException)
            {
                if (IsPlaylist)
                {
                    selectedSong = selectedPlaylist.GetSonglist()[0];
                }
                else
                {
                    selectedSong = songs[0];
                }
                playSong(selectedSong.SongLocation);
                playing = true;
                ButtonPlayPause.Content = FindResource("Pause");
                LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                LoadPlaylist();
                LoadSongs();
            }

        }

        private void ButtonSkipBackward_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            dispatcherTimer = new DispatcherTimer();
            musicplayer.currentTime = 0;
            timeSlider.Value = 0;
            try
            {
                if (selectedSong != null && !IsPlaylist)
                {
                    if (songs[songs.IndexOf(selectedSong) - 1] != null)
                    {
                        selectedSong = songs[songs.IndexOf(selectedSong) - 1];
                        playSong(selectedSong.SongLocation);
                        playing = true;
                        ButtonPlayPause.Content = FindResource("Pause");
                        LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                        LoadSongs();
                        LoadPlaylist();

                    }
                }
                else if (selectedSong != null && IsPlaylist)
                {
                    try
                    {
                        List<Song> songlist = selectedPlaylist.GetSonglist();
                        if (songlist[songlist.IndexOf(selectedSong) - 1] != null)
                        {
                            selectedSong = songlist[songlist.IndexOf(selectedSong) - 1];
                            playSong(selectedSong.SongLocation);
                            playing = true;
                            ButtonPlayPause.Content = FindResource("Pause");
                            LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                            LoadPlaylist();
                            LoadSongs();
                        }
                    } catch(NullReferenceException)
                    {
                        Console.WriteLine("No list selected");
                    }
                }
                else
                {
                    Console.WriteLine("No list selected");
                }
            }catch(ArgumentOutOfRangeException)
            {
                if (IsPlaylist)
                {
                    selectedSong = selectedPlaylist.GetSonglist().Last();
                }
                else
                {
                    selectedSong = songs.Last();
                }
                playSong(selectedSong.SongLocation);
                playing = true;
                ButtonPlayPause.Content = FindResource("Pause");
                LabelSongName.Content = selectedSong.SongName + " - " + selectedSong.Author;
                LoadPlaylist();
                LoadSongs();
            }
        }

        public void VolumeButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (!volumeMuted)
            {
                PreviousVolume = Volumeslider.Value;
                PreviousImage = volumebutton.Content;
                

                volumebutton.Content = FindResource("volume-0");
                Volumeslider.Value = 0;
                musicplayer.SetVolume(0);
                volumeMuted = true;
            }
            else if (volumeMuted)
            {
                volumebutton.Content = PreviousImage;
                Volumeslider.Value = PreviousVolume;
                musicplayer.SetVolume(PreviousVolume);
                volumeMuted = false;
            }
        }
        //####################################################################################################################################################################################
        #endregion

        #region OnMouseOverEvents
        //####################################################################################################################################################################################
        private void OnMouseEnterButtonPlayPause(object sender, EventArgs e)
        {
            if (!playing)
            {
                ButtonPlayPause.Content = FindResource("PlaySelect");
            }
            else
            {
                ButtonPlayPause.Content = FindResource("PauseSelect");
            }
        }
        private void OnMouseLeaveButtonPlayPause(object sender, EventArgs e)
        {
            if (!playing)
            {
                ButtonPlayPause.Content = FindResource("Play");
            }
            else
            {
                ButtonPlayPause.Content = FindResource("Pause");
            }
        }
        private void OnMouseEnterButtonSkipForward(object sender, MouseEventArgs e)
        {
            ButtonSkipForward.Content = FindResource("SkipForwardSelect");
            
        }
        private void OnMouseLeaveButtonSkipForward(object sender, MouseEventArgs e)
        {
            ButtonSkipForward.Content = FindResource("SkipForward");
        }
        private void OnMouseEnterButtonSkipBackward(object sender, MouseEventArgs e)
        {
            ButtonSkipBackward.Content = FindResource("SkipBackwardSelect");
        }
        private void OnMouseLeaveButtonSkipBackward(object sender, MouseEventArgs e)
        {
            ButtonSkipBackward.Content = FindResource("SkipBackward");
        }
        private void ButtonPlaylists_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!selectedPanel.Equals(PanelType.PlaylistsPanel))
            {
                ButtonPlaylists.Content = FindResource("PlaylistsSelect");
            }
        }

        private void ButtonPlaylists_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!selectedPanel.Equals(PanelType.PlaylistsPanel))
            {
                ButtonPlaylists.Content = FindResource("Playlists");
            }
        }

        private void ButtonHome_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!selectedPanel.Equals(PanelType.HomePanel))
            {
                ButtonHome.Content = FindResource("HomeSelect");
            }
        }

        private void ButtonHome_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!selectedPanel.Equals(PanelType.HomePanel))
            {
                ButtonHome.Content = FindResource("Home");
            }
        }

        private void ButtonSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!selectedPanel.Equals(PanelType.SearchPanel))
            {
                ButtonSearch.Content = FindResource("SearchSelect");
            }
        }

        private void ButtonSearch_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!selectedPanel.Equals(PanelType.SearchPanel))
            {
                ButtonSearch.Content = FindResource("Search");
            }
        }
        private void ButtonUser_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!selectedPanel.Equals(PanelType.UserPanel)) {
                ButtonUser.Content = FindResource("ProfileSelect");
            }
        }

        private void ButtonUser_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!selectedPanel.Equals(PanelType.UserPanel))
            {
                ButtonUser.Content = FindResource("Profile");
            }
        }


        //####################################################################################################################################################################################
        #endregion

        #region Searchbar methods
        //#########################################################################################
        
        /// <summary>
        /// Event for the search bar which calls when the text is changed in the search bar 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Searchbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadResults(Searchbar.Text);
        }
        
        /// <summary>
        /// Get the search results from the database which meet the input citeria
        /// </summary>
        /// <param name="result"></param>
        private void LoadResults(string result)
        {
            List<Song> searchresults = new List<Song>();
            songs = GetAllSongs();
            if (sortby.Equals("Artist"))
            {
                searchresults = songs.Where(x => x.Author.ToLower().Contains(result.ToLower())).OrderBy(x => x.Author).ToList();
            }
            else if (sortby.Equals("SongName"))
            {
                searchresults = songs.Where(x => x.SongName.ToLower().Contains(result.ToLower())).OrderBy(x => x.SongName).ToList();
            }
            songs = searchresults;
            LoadSongs();
            if (result != "" && searchresults.Count == 0)
            {
                RowDefinition SongListRow = new RowDefinition();
                SongListRow.Height = new GridLength(50);
                GridContent.RowDefinitions.Add(SongListRow);
                TextBlock EmptyMessage = new TextBlock();
                EmptyMessage.FontSize = 12;
                EmptyMessage.HorizontalAlignment = HorizontalAlignment.Center;
                EmptyMessage.Text = $"No result for '{Searchbar.Text}'";
                EmptyMessage.Foreground = new SolidColorBrush(Colors.LightGray);
                Grid.SetColumnSpan(EmptyMessage, 1);
                Grid.SetRow(EmptyMessage, 0);
                GridContent.Children.Add(EmptyMessage);
            }
        }

        /// <summary>
        /// Event for the Artist radio button to load the artist results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioArtist_Checked(object sender, RoutedEventArgs e)
        {
            sortby = "Artist";
            LoadResults(Searchbar.Text);
        }

        /// <summary>
        /// Event for the songname radio button to load the songname results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioSongname_Checked(object sender, RoutedEventArgs e)
        {
            sortby = "SongName";
            LoadResults(Searchbar.Text);
        }
        //############################################################################################
        #endregion

        #region User panel methods
        //#########################################################################################
        
        /// <summary>
        ///  loads the nessecary user information to show the user page
        /// </summary>
        public void LoadUserInfo()
        {
            currentUser = currentUser.loadCurrentUser(currentUser.UserID);
            HelloLabel_name.Content = currentUser.UserLastName;
            UserFirstname.Content = currentUser.UserFirstName;
            UserEmail.Content = currentUser.UserEmail;
            Username.Content = currentUser.UserLastName;
            if (currentUser.UserSubscription.Equals(1))
            {
                userSubscription.Content = "Subscribed";
            }
            else
            {
                userSubscription.Content = "None";
            }

            if (currentUser.UserSubscription.Equals(1))
            {
                grid_subscribe.Visibility = Visibility.Hidden;
            }
            else
            {
                grid_subscribe.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Event for the subscribe button, it calls the payment conformation window and makes the user subbed if successfull.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            Payment_confirm confirmPayment = new Payment_confirm(currentUser);
            confirmPayment.ShowDialog();

            if (confirmPayment.confirmation.Equals(1))
            {
                if (currentUser.SubscribeUser(currentUser.UserID))
                {
                    currentUser = currentUser.loadCurrentUser(currentUser.UserID);
                    LoadUserInfo();
                    grid_subscribe.Visibility = Visibility.Hidden;
                    Advertisement.Visibility = Visibility.Hidden;
                    Console.WriteLine("Subbed");
                }
                else
                {
                    Console.WriteLine("Not subbed");
                }
            }
            else
            {
                Console.WriteLine("Not subbed");
            }
        }
        //############################################################################################
        #endregion

        /// <summary>
        /// Event when the application is closed, it deletes any Guest uesers if avalible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (connection.DeleteUser(currentUser.UserID))
                {
                    currentUser = new User();
                    Console.WriteLine("Deleted");
                }
            }
            catch (Exception) {
                Console.WriteLine("No user was logged in");
            }
            Environment.Exit(0);
        }

        /// <summary>
        /// Event to update the timeslider while the song is playing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void updateTimeSlider(object sender, EventArgs e)
        {
            if (!MusicPlayer.paused)
            {
                if (musicplayer.length != 0)
                {
                    timeSlider.Maximum = musicplayer.length - 1;
                }
                changeTime = false;
                timeSlider.Value = musicplayer.currentTime;
                changeTime = true;
                if (musicplayer.currentTime % 60 < 10)
                {
                    labelCurrentTime.Content = musicplayer.currentTime / 60 + ":0" + musicplayer.currentTime % 60;
                }
                else
                {
                    labelCurrentTime.Content = musicplayer.currentTime / 60 + ":" + musicplayer.currentTime % 60;
                }
                if ((musicplayer.length - musicplayer.currentTime) % 60 < 10)
                {
                    labelTimeLeft.Content = (musicplayer.length - musicplayer.currentTime) / 60 + ":0" + (musicplayer.length - musicplayer.currentTime) % 60;
                }
                else
                {
                    labelTimeLeft.Content = (musicplayer.length - musicplayer.currentTime) / 60 + ":" + (musicplayer.length - musicplayer.currentTime) % 60;
                }
            }
        }

        /// <summary>
        /// Event for volume slider, if it has changed it calls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double slider;
            slider = Volumeslider.Value;
            musicplayer.SetVolume(slider);
            if (volumeMuted)
            {
                slider = PreviousVolume;
                volumeMuted = false; 
            }
            
            
            if (slider == 0)
            {
                volumebutton.Content = FindResource("volume-0");

            }
            else if (slider > 0 && slider <= 0.33)
            {
                volumebutton.Content = FindResource("volume-1");
            }
            else if (slider > 0.33 && slider <= 0.66)
            {
                volumebutton.Content = FindResource("volume-2");
            }
            else if (slider > 0.66 && slider < 1)
            {
                volumebutton.Content = FindResource("volume-3");
            }
        }

        private void TimeSlider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (changeTime)
            {
                musicplayer.jumpTime(Convert.ToInt32(timeSlider.Value));
                if (dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Stop();
                }
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(ButtonSkipForward_Clicker);
                if (musicplayer.length == 0 || musicplayer.currentTime == 0)
                {
                }
                else
                {
                    dispatcherTimer.Interval = TimeSpan.FromSeconds((musicplayer.length - musicplayer.currentTime));
                    dispatcherTimer.Start();
                }
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            selectedPanel = PanelType.SearchPanel;
            ButtonHome.Content = FindResource("Home");
            ButtonUser.Content = FindResource("Profile");
            ButtonPlaylists.Content = FindResource("Playlists");
            ButtonSearch.Content = FindResource("SearchActive");
            HomePanel.Visibility = Visibility.Hidden;
            PlaylistsPanel.Visibility = Visibility.Hidden;
            UserpagePanel.Visibility = Visibility.Hidden;
            SearchPanel.Visibility = Visibility.Visible;
            if (selectedPlaylist != null)
            {
                ScrollbarPlaylist.Visibility = Visibility.Visible;
                LabelPlaylistName.Visibility = Visibility.Visible;
            }
        }

        private void ButtonPlaylists_Click(object sender, RoutedEventArgs e)
        {
            selectedPanel = PanelType.PlaylistsPanel;
            ButtonHome.Content = FindResource("Home");
            ButtonUser.Content = FindResource("Profile");
            ButtonPlaylists.Content = FindResource("PlaylistsActive");
            ButtonSearch.Content = FindResource("Search");
            HomePanel.Visibility = Visibility.Hidden;
            SearchPanel.Visibility = Visibility.Hidden;
            UserpagePanel.Visibility = Visibility.Hidden;
            PlaylistsPanel.Visibility = Visibility.Visible;
            if(selectedPlaylist != null)
            {
                ScrollbarPlaylist.Visibility = Visibility.Visible;
                LabelPlaylistName.Visibility = Visibility.Visible;
            }
        }

        private void ButtonUser_Click(object sender, RoutedEventArgs e)
        {
            if (!currentUser.UserPassword.Equals("-"))
            {
                selectedPanel = PanelType.UserPanel;
                ButtonHome.Content = FindResource("Home");
                ButtonUser.Content = FindResource("ProfileActive");
                ButtonPlaylists.Content = FindResource("Playlists");
                ButtonSearch.Content = FindResource("Search");
                SearchPanel.Visibility = Visibility.Hidden;
                PlaylistsPanel.Visibility = Visibility.Hidden;
                ScrollbarPlaylist.Visibility = Visibility.Hidden;
                LabelPlaylistName.Visibility = Visibility.Hidden;
                HomePanel.Visibility = Visibility.Hidden;
                LoadUserInfo();
                UserpagePanel.Visibility = Visibility.Visible;
            } else
            {
                MessageBox.Show("You must create a profile to acces this feature");
            }
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            selectedPanel = PanelType.HomePanel;
            ButtonHome.Content = FindResource("HomeActive");
            ButtonUser.Content = FindResource("Profile");
            ButtonPlaylists.Content = FindResource("Playlists");
            ButtonSearch.Content = FindResource("Search");
            HomePanel.Visibility = Visibility.Visible;
            PlaylistsPanel.Visibility = Visibility.Hidden;
            UserpagePanel.Visibility = Visibility.Hidden;
            SearchPanel.Visibility = Visibility.Hidden;
            if (selectedPlaylist != null)
            {
                ScrollbarPlaylist.Visibility = Visibility.Visible;
                LabelPlaylistName.Visibility = Visibility.Visible;
            }
        }
    }
}
