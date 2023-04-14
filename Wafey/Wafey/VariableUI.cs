using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wafey
{
    class VariableUI
    {
        public enum ButtonType { Play, AddToPlaylist, AddToFavorite, RemoveFromPlaylist, RemovePlaylist, SelectPlaylist, SelectPlaylistFavorite, AddPlaylist}
        public enum TextblockType { AllSongInfo, SongLength, NameAndAuthor, AdminSongInfo }
        public enum TextboxType { AdminPanelRightTextbox }
        public enum LabelType { AdminPanelRightLabel }
        private Random rand = new Random();
        private int checkered = 0;
        public void CreateButton(MainWindow mainwindow, Grid grid, int rowcount, Song song, ButtonType buttontype, int height, int width, HorizontalAlignment horizontalalign, VerticalAlignment verticalalign, int rightpadding, bool isplaylist, Playlist playlist)
        {
            var ImageRemove = new ImageBrush();
            var ImageRemoveSelect = new ImageBrush();
            ImageRemove.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/RemoveButton.png"));
            ImageRemoveSelect.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/RemoveButtonSelect.png"));
            var ImageAdd = new ImageBrush();
            var ImageAddSelect = new ImageBrush();
            ImageAdd.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/AddButton.png"));
            ImageAddSelect.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/AddButtonSelect.png"));
            var AddFavorite = new ImageBrush();
            var AddSelectFavorite = new ImageBrush();
            AddFavorite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Favorite.png"));
            AddSelectFavorite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/FavoriteSelect.png"));
            switch (buttontype)
            {
                case ButtonType.Play:
                    var ImagePlay = new ImageBrush();
                    var ImagePlaySelect = new ImageBrush();
                    ImagePlay.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/PlayButton.png"));
                    ImagePlaySelect.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/PlayButtonSelect.png"));
                    Button ButtonPlaySong = new Button();
                    ButtonPlaySong.Background = ImagePlay;
                    ButtonPlaySong.Content = "";
                    ButtonPlaySong.HorizontalAlignment = horizontalalign;
                    ButtonPlaySong.VerticalAlignment = verticalalign;
                    ButtonPlaySong.Width = width;
                    ButtonPlaySong.Height = height;
                    ButtonPlaySong.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonPlaySong.Click += (s, e) =>
                    {
                        mainwindow.playSong(song.SongLocation);
                        mainwindow.playing = true;
                        mainwindow.ButtonPlayPause.Content = mainwindow.FindResource("Pause");
                        mainwindow.LabelSongName.Content = song.SongName + " - " + song.Author;
                        mainwindow.selectedSong = song;
                        mainwindow.IsPlaylist = isplaylist;
                        mainwindow.LoadPlaylist();
                        mainwindow.LoadSongs();
                    };
                    ButtonPlaySong.MouseEnter += (s, e) => { ButtonPlaySong.Background = ImagePlaySelect; };
                    ButtonPlaySong.MouseLeave += (s, e) => { ButtonPlaySong.Background = ImagePlay; };
                    Grid.SetRow(ButtonPlaySong, rowcount);
                    ButtonPlaySong.Name = "ButtonPlaySong" + rowcount.ToString();
                    grid.Children.Add(ButtonPlaySong);
                    break;

                case ButtonType.AddToPlaylist:
                    Button ButtonAddSong = new Button();
                    ButtonAddSong.Background = ImageAdd;
                    ButtonAddSong.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonAddSong.HorizontalAlignment = HorizontalAlignment.Right;
                    ButtonAddSong.VerticalAlignment = VerticalAlignment.Center;
                    ButtonAddSong.Height = 48;
                    ButtonAddSong.Width = 48;
                    ButtonAddSong.Click += (s, e) => {
                        if (mainwindow.selectedPlaylist != null)
                        {
                            if (!mainwindow.selectedPlaylist.GetSonglist().Contains(song))
                            {
                                //Console.WriteLine(song.SongName + " Added to playlist");
                                mainwindow.addPlaylistSong(mainwindow.selectedPlaylist.GetPlaylistID(), song.SongID);
                                //Console.WriteLine(mainwindow.selectedPlaylist.GetPlaylistID());
                                mainwindow.selectedPlaylist.GetSonglist().Add(song);
                                mainwindow.LoadPlaylist();
                            }
                            else
                            {
                                MessageBox.Show("Song is already present in playlist");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Select or create a playlist to add songs");
                        }
                    };
                    ButtonAddSong.MouseEnter += (s, e) => { ButtonAddSong.Background = ImageAddSelect; };
                    ButtonAddSong.MouseLeave += (s, e) => { ButtonAddSong.Background = ImageAdd; };
                    Grid.SetRow(ButtonAddSong, rowcount);
                    ButtonAddSong.Name = "ButtonAddSong" + rowcount.ToString();
                    grid.Children.Add(ButtonAddSong);
                    break;

                case ButtonType.AddToFavorite:
                    Button ButtonAddToFavorite = new Button();
                    if (mainwindow.favorite.HasSong(song))
                    {
                        ButtonAddToFavorite.Background = AddSelectFavorite;
                    }
                    else
                    {
                        ButtonAddToFavorite.Background = AddFavorite;
                    }
                    ButtonAddToFavorite.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonAddToFavorite.HorizontalAlignment = HorizontalAlignment.Right;
                    ButtonAddToFavorite.VerticalAlignment = VerticalAlignment.Center;
                    ButtonAddToFavorite.Height = 48;
                    ButtonAddToFavorite.Width = 48;
                    ButtonAddToFavorite.Margin = new Thickness(0, 0, rightpadding, 0);
                    ButtonAddToFavorite.Click += (s, e) => {

                        Button buttonAddFavorite = (Button)s;
                        
                        if (buttonAddFavorite.Background.Equals(AddSelectFavorite))
                        {
                            buttonAddFavorite.Background = AddFavorite;
                            mainwindow.removeFavoriteSong(mainwindow.currentUser.UserID, song.SongID);
                        }
                        else
                        {
                            mainwindow.addFavoriteSong(mainwindow.currentUser.UserID, song.SongID);
                            buttonAddFavorite.Background = AddSelectFavorite;
                        }

                        mainwindow.loadPlaylistsFromDB();
                        mainwindow.LoadPlaylists();
                        mainwindow.LoadPlaylist();
                        //Console.WriteLine(song.SongName);
                        
                    };
                    //ButtonAddToFavorite.MouseEnter += (s, e) => { ButtonAddToFavorite.Background = ImageAddSelect; };
                    //ButtonAddToFavorite.MouseLeave += (s, e) => { ButtonAddToFavorite.Background = ImageAdd; };
                    Grid.SetRow(ButtonAddToFavorite, rowcount);
                    ButtonAddToFavorite.Name = "ButtonAddFavorite" + rowcount.ToString();
                    grid.Children.Add(ButtonAddToFavorite);
                    break;

                case ButtonType.RemoveFromPlaylist:
                    Button ButtonRemoveSong = new Button();
                    ButtonRemoveSong.Background = ImageRemove;
                    ButtonRemoveSong.Content = "";
                    ButtonRemoveSong.HorizontalAlignment = HorizontalAlignment.Right;
                    ButtonRemoveSong.VerticalAlignment = VerticalAlignment.Center;
                    ButtonRemoveSong.Width = 30;
                    ButtonRemoveSong.Height = 30;
                    ButtonRemoveSong.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonRemoveSong.Click += (s, e) =>
                    {
                        if (mainwindow.selectedPlaylist.GetFavorite().Equals(1))
                        {
                            mainwindow.removeFavoriteSong(mainwindow.currentUser.UserID, song.SongID);
                            mainwindow.loadPlaylistsFromDB();
                            mainwindow.LoadPlaylists();
                            mainwindow.LoadSongs();
                        }
                        else
                        {
                            Console.WriteLine(song.SongName + " Removed from playlist");
                            mainwindow.removePlaylistSong(mainwindow.selectedPlaylist.GetPlaylistID(), song.SongID);
                            if (mainwindow.selectedSong == song)
                            {
                                mainwindow.selectedSong = null;
                            }
                            mainwindow.selectedPlaylist.GetSonglist().Remove(song);
                        }

                        mainwindow.LoadPlaylist();

                    };
                    ButtonRemoveSong.MouseEnter += (s, e) => { ButtonRemoveSong.Background = ImageRemoveSelect; };
                    ButtonRemoveSong.MouseLeave += (s, e) => { ButtonRemoveSong.Background = ImageRemove; };
                    Grid.SetRow(ButtonRemoveSong, rowcount);
                    ButtonRemoveSong.Name = "ButtonRemoveSong" + rowcount.ToString();
                    grid.Children.Add(ButtonRemoveSong);
                    break;

                case ButtonType.RemovePlaylist:
                    Button ButtonRemovePlaylist = new Button();
                    ButtonRemovePlaylist.Background = ImageRemove;
                    ButtonRemovePlaylist.Content = "";
                    ButtonRemovePlaylist.HorizontalAlignment = HorizontalAlignment.Right;
                    ButtonRemovePlaylist.VerticalAlignment = VerticalAlignment.Center;
                    ButtonRemovePlaylist.Width = 19;
                    ButtonRemovePlaylist.Height = 19;
                    ButtonRemovePlaylist.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonRemovePlaylist.Click += (s, e) =>
                    {
                        var dialog = new RemovePlaylistConfirmationWindow(playlist.GetName());
                        if (dialog.ShowDialog() == true)
                        {
                            if (dialog.DialogResult == true)
                            {
                                if (mainwindow.selectedPlaylist == playlist)
                                {
                                    mainwindow.selectedPlaylist = null;
                                }
                                mainwindow.removePlaylist(playlist.GetPlaylistID());
                                mainwindow.playlists.Remove(playlist);
                                Console.WriteLine("Playlist " + playlist.GetName() + " has been removed");
                                mainwindow.LoadPlaylists();
                                mainwindow.LoadPlaylist();
                            }
                        }
                    };
                    ButtonRemovePlaylist.MouseEnter += (s, e) => { ButtonRemovePlaylist.Background = ImageRemoveSelect; };
                    ButtonRemovePlaylist.MouseLeave += (s, e) => { ButtonRemovePlaylist.Background = ImageRemove; };
                    Grid.SetRow(ButtonRemovePlaylist, rowcount);
                    ButtonRemovePlaylist.Name = "ButtonRemoveSong" + rowcount.ToString();
                    grid.Children.Add(ButtonRemovePlaylist);
                    break;

                case ButtonType.SelectPlaylist:
                    Button ButtonSelectPlaylist = new Button();
                    ButtonSelectPlaylist.Content = playlist.GetName();
                    ButtonSelectPlaylist.Foreground = new SolidColorBrush(Colors.LightGray);
                    ButtonSelectPlaylist.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonSelectPlaylist.Click += (s, e) => {
                        mainwindow.selectedPlaylist = playlist;
                        mainwindow.LoadPlaylist();
                    };
                    Grid.SetRow(ButtonSelectPlaylist, rowcount);
                    ButtonSelectPlaylist.Name = "ButtonSelectPlaylist" + rowcount.ToString();
                    grid.Children.Add(ButtonSelectPlaylist);
                    break;
            }
        }
        public void CreatePlaylistButton(MainWindow mainwindow, Grid grid, Playlist playlist, int rowcount, int columncount, ButtonType buttontype)
        {
            switch (buttontype) {
                case ButtonType.AddPlaylist:
                    var ImageAdd = new ImageBrush();
                    var ImageAddSelect = new ImageBrush();
                    ImageAdd.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/AddButton.png"));
                    ImageAddSelect.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/AddButtonSelect.png"));
                    Button AddPlaylist = new Button();
                    Label Addplaylisttext = new Label();
                    Addplaylisttext.Content = "Create new playlist";
                    Addplaylisttext.FontSize = 20;
                    Addplaylisttext.Foreground = new SolidColorBrush(Colors.Gray);
                    Addplaylisttext.HorizontalAlignment = HorizontalAlignment.Center;
                    AddPlaylist.Background = ImageAdd;
                    AddPlaylist.Content = "";
                    AddPlaylist.HorizontalAlignment = HorizontalAlignment.Center;
                    AddPlaylist.VerticalAlignment = VerticalAlignment.Center;
                    AddPlaylist.Width = 50;
                    AddPlaylist.Height = 50;
                    AddPlaylist.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    AddPlaylist.Click += (s, e) =>
                    {
                        var dialog = new PlaylistNameDialog(mainwindow.playlists);
                        if (dialog.ShowDialog() == true)
                        {
                            Playlist newplaylist = new Playlist(dialog.GetPlaylistName , 0);
                            mainwindow.addPlaylist(newplaylist.GetName(), mainwindow.currentUser.UserID);
                            mainwindow.loadPlaylistsFromDB();
                            mainwindow.LoadPlaylists();
                            foreach (Playlist playlist_0 in mainwindow.playlists)
                            {
                                if (newplaylist.GetName() == playlist_0.GetName())
                                {
                                    mainwindow.selectedPlaylist = playlist_0;
                                }
                            }
                            mainwindow.LoadPlaylist();
                        }
                    };
                    AddPlaylist.MouseEnter += (s, e) => { AddPlaylist.Background = ImageAddSelect; };
                    AddPlaylist.MouseLeave += (s, e) => { AddPlaylist.Background = ImageAdd; };
                    Grid.SetRow(Addplaylisttext, rowcount);
                    Grid.SetColumn(Addplaylisttext, columncount);
                    grid.Children.Add(Addplaylisttext);
                    Grid.SetRow(AddPlaylist, rowcount);
                    Grid.SetColumn(AddPlaylist, columncount);
                    AddPlaylist.Name = "ButtonRemoveSong" + rowcount.ToString();
                    grid.Children.Add(AddPlaylist);
                    break;
                case ButtonType.SelectPlaylist:
                    var ImageRemove = new ImageBrush();
                    var ImageRemoveSelect = new ImageBrush();
                    ImageRemove.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/RemoveButton.png"));
                    ImageRemoveSelect.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/RemoveButtonSelect.png"));
                    Grid playlistblock = new Grid();
                    Button ButtonRemovePlaylist = new Button();
                    ButtonRemovePlaylist.Background = ImageRemove;
                    ButtonRemovePlaylist.Content = "";
                    ButtonRemovePlaylist.HorizontalAlignment = HorizontalAlignment.Right;
                    ButtonRemovePlaylist.VerticalAlignment = VerticalAlignment.Top;
                    ButtonRemovePlaylist.Width = 30;
                    ButtonRemovePlaylist.Height = 30;
                    ButtonRemovePlaylist.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonRemovePlaylist.Click += (s, e) =>
                    {
                        var dialog = new RemovePlaylistConfirmationWindow(playlist.GetName());
                        if (dialog.ShowDialog() == true)
                        {
                            if (dialog.DialogResult == true)
                            {
                                if (mainwindow.selectedPlaylist == playlist)
                                {
                                    mainwindow.selectedPlaylist = null;
                                }
                                mainwindow.removePlaylist(playlist.GetPlaylistID());
                                mainwindow.playlists.Remove(playlist);
                                Console.WriteLine("Playlist " + playlist.GetName() + " has been removed");
                                mainwindow.LoadPlaylists();
                                mainwindow.LoadPlaylist();
                            }
                        }
                    };
                    ButtonRemovePlaylist.MouseEnter += (s, e) => { ButtonRemovePlaylist.Background = ImageRemoveSelect; };
                    ButtonRemovePlaylist.MouseLeave += (s, e) => { ButtonRemovePlaylist.Background = ImageRemove; };
                    ButtonRemovePlaylist.Name = "ButtonRemoveSong" + rowcount.ToString();

                    playlistblock.MouseLeftButtonDown += (s, e) =>
                    {
                        mainwindow.selectedPlaylist = playlist;
                        mainwindow.LoadPlaylist();
                        mainwindow.LoadSongs();
                    };
                    
                    Button ButtonSelectPlaylist = new Button();
                    ButtonSelectPlaylist.Content = playlist.GetName();
                    ButtonSelectPlaylist.Foreground = new SolidColorBrush(Colors.LightGray);
                    ButtonSelectPlaylist.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonSelectPlaylist.HorizontalAlignment = HorizontalAlignment.Center;
                    ButtonSelectPlaylist.VerticalAlignment = VerticalAlignment.Center;
                    ButtonSelectPlaylist.FontSize = 20;
                    ButtonSelectPlaylist.Click += (s, e) =>
                    {
                        mainwindow.selectedPlaylist = playlist;
                        mainwindow.LoadPlaylist();
                        mainwindow.LoadSongs();
                    };
                    ButtonSelectPlaylist.Name = "ButtonSelectPlaylist" + rowcount.ToString();

                    playlistblock.Children.Add(ButtonSelectPlaylist);
                    playlistblock.Children.Add(ButtonRemovePlaylist);
                    //playlistblock.Background = new SolidColorBrush(Colors.Black);
                    LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                    myLinearGradientBrush.StartPoint = new Point(1, 0);
                    myLinearGradientBrush.EndPoint = new Point(0, 1);
                    myLinearGradientBrush.GradientStops.Add(
                        new GradientStop(Color.FromArgb(50, 30, 30, 30), 0.0));
                    myLinearGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(70, 30, 30, 30), 0.20));
                    if (checkered == 0) {
                        myLinearGradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(20, 80, 200), 0.20));
                        checkered = 1;
                    } else
                    {
                        myLinearGradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(20, 10, 200), 0.20));
                        checkered = 0;
                    }
                    myLinearGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(70, 30, 30, 30), 0.80));
                    playlistblock.Background = myLinearGradientBrush;
                    playlistblock.Margin = new Thickness(5, 5, 5, 5);
                    Grid.SetRow(playlistblock, rowcount);
                    Grid.SetColumn(playlistblock, columncount);
                    grid.Children.Add(playlistblock);
                    break;
                case ButtonType.SelectPlaylistFavorite:
                    Grid playlistFavoriteblock = new Grid();
                    
                    playlistFavoriteblock.MouseLeftButtonDown += (s, e) =>
                    {
                        mainwindow.selectedPlaylist = playlist;
                        mainwindow.LoadPlaylist();
                        mainwindow.LoadSongs();
                    };

                    Button ButtonSelectPlaylistFavorite = new Button();
                    ButtonSelectPlaylistFavorite.Content = mainwindow.FindResource("Favorite");
                    ButtonSelectPlaylistFavorite.Foreground = new SolidColorBrush(Colors.LightGray);
                    ButtonSelectPlaylistFavorite.Style = (Style)mainwindow.FindResource("RemoveMouseover");
                    ButtonSelectPlaylistFavorite.HorizontalAlignment = HorizontalAlignment.Center;
                    ButtonSelectPlaylistFavorite.VerticalAlignment = VerticalAlignment.Center;
                    ButtonSelectPlaylistFavorite.FontSize = 20;
                    ButtonSelectPlaylistFavorite.Click += (s, e) =>
                    {
                        mainwindow.selectedPlaylist = playlist;
                        mainwindow.LoadPlaylist();
                        mainwindow.LoadSongs();
                    };
                    ButtonSelectPlaylistFavorite.Name = "ButtonSelectPlaylist" + rowcount.ToString();

                    playlistFavoriteblock.Children.Add(ButtonSelectPlaylistFavorite);
                    //playlistblock.Background = new SolidColorBrush(Colors.Black);
                    LinearGradientBrush myLinearGradientBrushFavorite = new LinearGradientBrush();
                    myLinearGradientBrushFavorite.StartPoint = new Point(1, 0);
                    myLinearGradientBrushFavorite.EndPoint = new Point(0, 1);
                    myLinearGradientBrushFavorite.GradientStops.Add(
                        new GradientStop(Color.FromArgb(50, 30, 30, 30), 0.0));
                    myLinearGradientBrushFavorite.GradientStops.Add(new GradientStop(Color.FromArgb(70, 30, 30, 30), 0.20));

                    if (mainwindow.playlists.Count == 1)
                    {
                        myLinearGradientBrushFavorite.GradientStops.Add(new GradientStop(Color.FromRgb(20, 80, 200), 0.20));
                        checkered = 1;
                    }
                    else
                    {
                        if (checkered == 0)
                        {
                            myLinearGradientBrushFavorite.GradientStops.Add(new GradientStop(Color.FromRgb(20, 80, 200), 0.20));
                            checkered = 1;
                        }
                        else
                        {
                            myLinearGradientBrushFavorite.GradientStops.Add(new GradientStop(Color.FromRgb(20, 10, 200), 0.20));
                            checkered = 0;
                        }
                    }
                    myLinearGradientBrushFavorite.GradientStops.Add(new GradientStop(Color.FromArgb(70, 30, 30, 30), 0.80));
                    playlistFavoriteblock.Background = myLinearGradientBrushFavorite;
                    playlistFavoriteblock.Margin = new Thickness(5, 5, 5, 5);
                    Grid.SetRow(playlistFavoriteblock, rowcount);
                    Grid.SetColumn(playlistFavoriteblock, columncount);
                    grid.Children.Add(playlistFavoriteblock);
                    break;
            }

        }
        public void CreateTextBlock(MainWindow window, Grid grid, Song song, int rowcount, TextblockType textblocktype, int leftpadding, int rightpadding)
        {
            TextBlock SongInfo = new TextBlock();
            switch (textblocktype)
            {
                case TextblockType.AllSongInfo:

                    SongInfo.FontSize = 12;
                    if (window.selectedSong == song && window.IsPlaylist)
                    {
                        SongInfo.Foreground = new SolidColorBrush(Colors.CadetBlue);
                    }
                    else
                    {
                        SongInfo.Foreground = new SolidColorBrush(Colors.LightGray);
                    }
                    SongInfo.Margin = new Thickness(leftpadding, 0, rightpadding, 0);
                    SongInfo.HorizontalAlignment = HorizontalAlignment.Left;
                    Grid.SetColumnSpan(SongInfo, 1);
                    if ((song.Length % 60) < 10)
                    {
                        SongInfo.Text = $"{song.SongName}\nBy: {song.Author} \n{song.Length / 60}:0{song.Length % 60}";
                    }
                    else
                    {
                        SongInfo.Text = $"{song.SongName}\nBy: {song.Author} \n{song.Length / 60}:{song.Length % 60}";
                    }
                    SongInfo.Name = "Song" + song.SongID;
                    Grid.SetRow(SongInfo, rowcount);
                    grid.Children.Add(SongInfo);

                    break;

                case TextblockType.NameAndAuthor:
                    SongInfo.FontSize = 20;
                    SongInfo.Margin = new Thickness(leftpadding, 0, rightpadding, 0);
                    SongInfo.Foreground = new SolidColorBrush(Colors.LightGray);
                    if (window.selectedSong != null)
                    {
                        if (window.selectedSong.SongID == song.SongID && !window.IsPlaylist)
                        {
                            SongInfo.Foreground = new SolidColorBrush(Colors.CadetBlue);
                        }
                    }
                    if (window.Searchbar.Text != "" && window.sortby == "SongName")
                    {
                        string searchresult = window.Searchbar.Text;
                        string songname = song.SongName;
                        string lowerCaseSongName = songname.ToLower();
                        string lowerCaseSearchResult = searchresult.ToLower();
                        int resultlength = searchresult.Length;
                        int songlength = songname.Length;


                        bool isPresent = false;

                        for (int i = 0; i < songname.Length; i++)
                        {
                            if (resultlength == 1)
                            {
                                if (lowerCaseSongName[i] == lowerCaseSearchResult[0])
                                {
                                    SongInfo.Inlines.Add(new Run(songname[i].ToString()) { Background = Brushes.Blue });
                                }
                                else
                                {
                                    SongInfo.Inlines.Add(new Run(songname[i].ToString()));
                                }
                            }
                            else if (resultlength > 1 && lowerCaseSongName[i] == lowerCaseSearchResult[0] && (resultlength + i) <= songlength)
                            {
                                StringBuilder sb = new StringBuilder();
                                for (int j = 0; j < resultlength; j++)
                                {
                                    if (lowerCaseSearchResult[j] == lowerCaseSongName[i + j])
                                    {
                                        isPresent = true;
                                        sb.Append(songname[i + j]);
                                    }
                                    else
                                    {
                                        isPresent = false;
                                        sb.Clear();
                                        break;
                                    }
                                }
                                if (isPresent == true)
                                {
                                    SongInfo.Inlines.Add(new Run(sb.ToString()) { Background = Brushes.Blue });
                                    i = i + (resultlength - 1);
                                }
                                else
                                {
                                    SongInfo.Inlines.Add(new Run(songname[i].ToString()));
                                }
                            }
                            else
                            {
                                SongInfo.Inlines.Add(new Run(songname[i].ToString()));
                            }
                        }
                        SongInfo.Inlines.Add(" By: ");
                        SongInfo.Inlines.Add(song.Author);
                    }
                    else if (window.Searchbar.Text != "" && window.sortby == "Artist")
                    {
                        SongInfo.Inlines.Add(song.SongName);
                        SongInfo.Inlines.Add(" By: ");
                        string searchresult = window.Searchbar.Text;
                        string songartist = song.Author;
                        string lowerCaseSongArtist = songartist.ToLower();
                        string lowerCaseSearchResult = searchresult.ToLower();
                        int resultlength = searchresult.Length;
                        int songlength = songartist.Length;


                        bool isPresent = false;

                        for (int i = 0; i < songartist.Length; i++)
                        {
                            if (resultlength == 1)
                            {
                                if (lowerCaseSongArtist[i] == lowerCaseSearchResult[0])
                                {
                                    SongInfo.Inlines.Add(new Run(songartist[i].ToString()) { Background = Brushes.Blue });
                                }
                                else
                                {
                                    SongInfo.Inlines.Add(new Run(songartist[i].ToString()));
                                }
                            }
                            else if (resultlength > 1 && lowerCaseSongArtist[i] == lowerCaseSearchResult[0] && (resultlength + i) <= songlength)
                            {
                                StringBuilder sb = new StringBuilder();
                                for (int j = 0; j < resultlength; j++)
                                {
                                    if (lowerCaseSearchResult[j] == lowerCaseSongArtist[i + j])
                                    {
                                        isPresent = true;
                                        sb.Append(songartist[i + j]);
                                    }
                                    else
                                    {
                                        isPresent = false;
                                        sb.Clear();
                                        break;
                                    }
                                }
                                if (isPresent == true)
                                {
                                    SongInfo.Inlines.Add(new Run(sb.ToString()) { Background = Brushes.Blue });
                                    i = i + (resultlength - 1);
                                }
                                else
                                {
                                    SongInfo.Inlines.Add(new Run(songartist[i].ToString()));
                                }
                            }
                            else
                            {
                                SongInfo.Inlines.Add(new Run(songartist[i].ToString()));
                            }
                        }

                    } else
                    {
                        SongInfo.Text = $"{song.SongName} By: {song.Author}";
                    }
                    SongInfo.HorizontalAlignment = HorizontalAlignment.Left;
                    SongInfo.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetColumnSpan(SongInfo, 1);
                    //SongInfo.Text = $"{song.SongName} By: {song.Author}";
                    Grid.SetRow(SongInfo, rowcount);
                    SongInfo.Name = "SongInfo" + rowcount.ToString();
                    grid.Children.Add(SongInfo);
                    break;

                case TextblockType.SongLength:
                    TextBlock SongInfo2 = new TextBlock();
                    SongInfo2.FontSize = 20;
                    SongInfo2.Margin = new Thickness(0, 0, rightpadding, 0);
                    SongInfo2.Foreground = new SolidColorBrush(Colors.LightGray);
                    if (window.selectedSong != null)
                    {
                        if (window.selectedSong.SongID == song.SongID && !window.IsPlaylist)
                        {
                            SongInfo2.Foreground = new SolidColorBrush(Colors.CadetBlue);
                        }
                    }
                    SongInfo2.HorizontalAlignment = HorizontalAlignment.Right;
                    SongInfo2.VerticalAlignment = VerticalAlignment.Center;
                    SongInfo2.Name = "SongInfo2" + rowcount.ToString();
                    Grid.SetColumnSpan(SongInfo2, 1);
                    if ((song.Length % 60) < 10)
                    {
                        SongInfo2.Text = $"{song.Length / 60}:0{song.Length % 60}";
                    }
                    else
                    {
                        SongInfo2.Text = $"{song.Length / 60}:{song.Length % 60}";
                    }
                    Grid.SetRow(SongInfo2, rowcount);
                    grid.Children.Add(SongInfo2);
                    break;
            }
        }
        public void CreateTextBlock(AdminPanel window, Grid grid, Song song, int rowcount, int columnCount, TextblockType textblocktype, int leftpadding, int rightpadding, string search)
        {

            TextBlock SongInfo = new TextBlock();
            switch (textblocktype)
            {
                case TextblockType.AdminSongInfo:
                    SongInfo.FontSize = 12;
                    SongInfo.Margin = new Thickness(leftpadding, 0, rightpadding, 0);
                    SongInfo.Foreground = new SolidColorBrush(Colors.White);
                    SongInfo.HorizontalAlignment = HorizontalAlignment.Left;

                    if (search != null)
                    {

                        colorTextBetween(ref SongInfo, song.SongName, search, 1);
                        colorTextBetween(ref SongInfo, song.Author, search, 0);

                    }
                    else
                    {
                        SongInfo.Inlines.Add($"{song.SongName}\nBy: {song.Author}");
                    }

                    if ((song.Length % 60) < 10)
                    {
                        SongInfo.Inlines.Add($"\n{song.Length / 60}:0{song.Length % 60}");
                    }
                    else
                    {
                        SongInfo.Inlines.Add($"\n{song.Length / 60}:{song.Length % 60}");
                    }

                    SongInfo.PreviewMouseDown += (s, e) =>
                    {
                        TextBlock songPressed = (TextBlock)s;
                        window.CurrentElementID = int.Parse(songPressed.Name.Replace("Song", "").ToString());
                        window.LoadRightPanel();

                        if(window.DeleteElement.Visibility == Visibility.Hidden && window.DeleteElementLabel.Visibility == Visibility.Hidden && window.ChangeElement.Visibility == Visibility.Hidden && window.ChangeElementLabel.Visibility == Visibility.Hidden)
                        {
                            window.DeleteElement.Visibility = Visibility.Visible;
                            window.DeleteElementLabel.Visibility = Visibility.Visible;
                            window.ChangeElement.Visibility = Visibility.Visible;
                            window.ChangeElementLabel.Visibility = Visibility.Visible;
                        }

                    };
                    SongInfo.Name = "Song" + song.SongID.ToString();
                    Grid.SetColumnSpan(SongInfo, columnCount);
                    Grid.SetRow(SongInfo, rowcount);
                    grid.Children.Add(SongInfo);
                    break;

            }
        }
        public void CreateTextBlock(AdminPanel window, Grid grid, User user, int rowcount, int columnCount, TextblockType textblocktype, int leftpadding, int rightpadding, string search)
        {

            TextBlock UserInfo = new TextBlock();
            switch (textblocktype)
            {
                case TextblockType.AdminSongInfo:
                    UserInfo.FontSize = 12;
                    UserInfo.Margin = new Thickness(leftpadding, 0, rightpadding, 0);
                    UserInfo.Foreground = new SolidColorBrush(Colors.White);
                    UserInfo.HorizontalAlignment = HorizontalAlignment.Left;

                    if (search != null)
                    {

                        colorTextBetween(ref UserInfo, user.UserFirstName, search, 1);
                        colorTextBetween(ref UserInfo, user.UserEmail, search, 0);

                    }
                    else
                    {
                        UserInfo.Inlines.Add($"{user.UserFirstName}\nBy: {user.UserEmail}");
                    }

                    UserInfo.PreviewMouseDown += (s, e) =>
                    {
                        TextBlock songPressed = (TextBlock)s;
                        window.CurrentElementID = int.Parse(songPressed.Name.Replace("User", "").ToString());
                        window.LoadRightPanel();

                        if (window.DeleteElement.Visibility == Visibility.Hidden && window.DeleteElementLabel.Visibility == Visibility.Hidden && window.ChangeElement.Visibility == Visibility.Hidden && window.ChangeElementLabel.Visibility == Visibility.Hidden)
                        {
                            window.DeleteElement.Visibility = Visibility.Visible;
                            window.DeleteElementLabel.Visibility = Visibility.Visible;
                            window.ChangeElement.Visibility = Visibility.Visible;
                            window.ChangeElementLabel.Visibility = Visibility.Visible;
                        }

                    };
                    UserInfo.Name = "User" + user.UserID.ToString();
                    Grid.SetColumnSpan(UserInfo, columnCount);
                    Grid.SetRow(UserInfo, rowcount);
                    grid.Children.Add(UserInfo);
                    break;

            }
        }
        public void CreateTextBox(AdminPanel window, Grid grid, string elementTextboxValue, string textboxName, int rowcount, int columnCount, TextboxType textboxtype, int leftpadding, int rightpadding, bool elementActive)
        {
            TextBox elementLabelTextBox = new TextBox();
            switch (textboxtype)
            {
                case TextboxType.AdminPanelRightTextbox:
                    elementLabelTextBox.FontSize = 12;
                    elementLabelTextBox.Foreground = new SolidColorBrush(Colors.Black);
                    elementLabelTextBox.Margin = new Thickness(leftpadding, 0, rightpadding, 0);
                    elementLabelTextBox.Width = 350;
                    elementLabelTextBox.Height = 20;
                    if(textboxName != null)
                    {
                        elementLabelTextBox.Name = textboxName;
                    }
                    elementLabelTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                    elementLabelTextBox.IsEnabled = elementActive;

                    elementLabelTextBox.Text = elementTextboxValue;

                    Grid.SetColumnSpan(elementLabelTextBox, columnCount);
                    Grid.SetRow(elementLabelTextBox, rowcount);

                    RowDefinition SongTextboxElementRow = new RowDefinition();
                    SongTextboxElementRow.Height = new GridLength(30);
                    grid.RowDefinitions.Add(SongTextboxElementRow);
                    grid.Children.Add(elementLabelTextBox);
                    break;
            }
        }
        public void CreateLabel(AdminPanel window, Grid grid, string labelName, int rowcount, int columnCount, LabelType labelType, int leftpadding, int rightpadding)
        {
            Label elementLabel = new Label();
            switch (labelType)
            {
                case LabelType.AdminPanelRightLabel:

                    elementLabel.FontSize = 12;
                    elementLabel.Foreground = new SolidColorBrush(Colors.White);
                    elementLabel.Margin = new Thickness(leftpadding, 0, rightpadding, 0);
                    elementLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    elementLabel.Content = labelName;

                    Grid.SetColumnSpan(elementLabel, columnCount);
                    Grid.SetRow(elementLabel, rowcount);

                    RowDefinition SongElementRow = new RowDefinition();
                    SongElementRow.Height = new GridLength(30);
                    grid.RowDefinitions.Add(SongElementRow);
                    grid.Children.Add(elementLabel);
                    break;
            }
        }
        public void colorTextBetween(ref TextBlock textboxColor, string stringToSplit, string part, int by)
        {

            string searchresult = part;
            string songname = stringToSplit;
            string lowerCaseSongName = songname.ToLower();
            string lowerCaseSearchResult = searchresult.ToLower();
            int resultlength = searchresult.Length;
            int songlength = songname.Length;


            bool isPresent = false;

            for (int i = 0; i < songname.Length; i++)
            {
                if (resultlength == 1)
                {
                    if (lowerCaseSongName[i] == lowerCaseSearchResult[0])
                    {
                        textboxColor.Inlines.Add(new Run(songname[i].ToString()) { Background = Brushes.Blue });
                    }
                    else
                    {
                        textboxColor.Inlines.Add(new Run(songname[i].ToString()));
                    }
                }
                else if (resultlength > 1 && lowerCaseSongName[i] == lowerCaseSearchResult[0] && (resultlength + i) <= songlength)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < resultlength; j++)
                    {
                        if (lowerCaseSearchResult[j] == lowerCaseSongName[i + j])
                        {
                            isPresent = true;
                            sb.Append(songname[i + j]);
                        }
                        else
                        {
                            isPresent = false;
                            sb.Clear();
                            break;
                        }
                    }
                    if (isPresent == true)
                    {
                        textboxColor.Inlines.Add(new Run(sb.ToString()) { Background = Brushes.Blue });
                        i = i + (resultlength - 1);
                    }
                    else
                    {
                        textboxColor.Inlines.Add(new Run(songname[i].ToString()));
                    }
                }
                else
                {
                    textboxColor.Inlines.Add(new Run(songname[i].ToString()));
                }
            }
            if(by == 1)
            {
                textboxColor.Inlines.Add(new Run("\nBy: "));
            }
        }
    }
}
