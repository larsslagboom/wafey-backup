using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wafey
{
    public class Playlist
    {
        [JsonProperty("PlaylistName")]
        private string PlaylistName;
        [JsonProperty("Songlist")]
        private List<Song> Songlist = new List<Song>();
        [JsonProperty("PlaylistID")]
        private int PlaylistID;
        private int Favorite = 0;

        public Playlist()
        {
            Favorite = 0;
        }

        public Playlist(string name, int favorite)
        {
            this.PlaylistName = name;
            this.Favorite = favorite;
        }
        public void RemoveSong(Song song)
        {
            Songlist.Remove(song);
        }
        public Boolean HasSong(Song song)
        {
            return this.Songlist.Where(s => s.SongID.Equals(song.SongID)).Count() > 0;
        }
        public void AddSong(Song song)
        {
            Songlist.Add(song);
        }
        public void SetSonglist(List<Song> songs)
        {
            this.Songlist = songs;
        }
        public List<Song> GetSonglist()
        {
            return Songlist;
        }
        public string GetName()
        {
            return PlaylistName;
        }
        public int GetFavorite()
        {
            return Favorite;
        }
        public int GetPlaylistID()
        {
            return PlaylistID;
        }
    }
}
