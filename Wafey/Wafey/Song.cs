using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wafey
{
    public class Song
    {
        [JsonProperty("PlaylistsongID")]
        public int PlaylistsongID { get; set; }
        [JsonProperty("SongID")]
        public int SongID { get; set; }
        [JsonProperty("SongName")]
        public string SongName { get; set; }
        [JsonProperty("Author")]
        public string Author { get; set; }
        [JsonProperty("SongLength")]
        public int Length { get; set; }
        [JsonProperty("Genre")]
        public string Genre { get; set; }
        [JsonProperty("SongLocation")]
        public string SongLocation { get; set; }
        public Song(string name,string author,int length,string link)
        {
            this.SongName = name;
            this.Author = author;
            this.Length = length;
            this.SongLocation = link;
        }
    }
}
