using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wine
{
    class Artist
    {
        public string Name { get; set; }
        public List<Album> Albums { get; set; }

        public Artist()
        {
            Albums = new List<Album>();
        }

    }
    class Song
    {
        public string Title { get; set; }
        public Album Album { get; set; }
        public Artist Artist { get; set; }
        public string FilePath { get; set; }
    }
    class Album
    {
        public string Name { get; set; }
        public Artist Artist { get; set; }
        public List<Song> Songs { get; set; }
        public BitmapImage AlbumArt { get; set; }
        public Album()
        {
            Songs = new List<Song>();
        }
    }
}
