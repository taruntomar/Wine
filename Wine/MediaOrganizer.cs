using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TagLib;
using Wine.TreeViewModel;

namespace Wine
{
    public partial class MainWindow : Window
    {
        private void ChangeDirectory_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();


            DirectoryAddressTextBox.Text = fbd.SelectedPath;
            Refresh_Click(null, null);
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoryAddressTextBox.Text.Length == 0)
                return;

            m_files = Directory.GetFiles(DirectoryAddressTextBox.Text);
            m_directories = Directory.GetDirectories(DirectoryAddressTextBox.Text);

            if (m_files.Length == 0 && m_directories.Length == 0)
                return;
            m_worker = new BackgroundWorker();
            m_worker.WorkerReportsProgress = true;
            m_worker.DoWork += worker_DoWork;
            m_worker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            m_worker.RunWorkerCompleted += m_worker_RunWorkerCompleted;
            m_worker.RunWorkerAsync();
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            m_artistlist = new List<Artist>();
            m_albumslist = new List<Album>();
            m_songslist = new List<Song>();
            string[] files = null;
            for (int i = 0; i <= m_directories.Length; i++)
            {

                if (i == 0)
                    files = m_files;
                else
                    files = Directory.GetFiles(m_directories[i - 1]);

                foreach (string file in files)
                {
                    string x = System.IO.Path.GetExtension(file);
                    if (x == ".mp3")
                    {
                        TagLib.File tlFile;
                        try
                        {
                            tlFile = TagLib.File.Create(file);
                            Song song = new Song();
                            song.Title = tlFile.Tag.Title; // get song  title 
                            song.FilePath = file; // get file path

                            //////////******** GET ARTIST *******///////
                            string artistname = tlFile.Tag.Performers[0]; // get artist name
                            Artist artist = GetArtist(artistname);
                            if (artist == null)
                            {
                                artist = new Artist() { Name = artistname };
                                m_artistlist.Add(artist);
                            }
                            song.Artist = artist;
                           
                            //////////******** END OF GET ARTIST *******///////

                            //////////******** GET ALBUM *******///////
                            string albumname = tlFile.Tag.Performers[0]; // get album name
                            Album album = GetAlbum(albumname);
                            if (album == null)
                            {
                                album = new Album() { Name = albumname };
                                m_albumslist.Add(album);
                                
                            }
                            song.Album = album;
                            album.Songs.Add(song);
                            if (!artist.Albums.Contains(album))
                                artist.Albums.Add(album);
                            //////////******** END OF GET ALBUM *******///////

                            m_songslist.Add(song);
                        }
                        catch (Exception ex) { }
                    }
                }
            }
            m_worker.ReportProgress(100);
        }
        private Album GetAlbum(string albumname)
        {
            var x = from tmpalbum in m_albumslist where tmpalbum.Name == albumname select tmpalbum;
            if (x.Count() == 0)
                return null;
            return x.First();
        }
        private Artist GetArtist(string artistname)
        {
            var x = from tmpartist in m_artistlist where tmpartist.Name == artistname select tmpartist;

            if (x.Count() == 0)
                return null;
            return x.First();
        }
        void m_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            processbar.Value = 0;
            LoadArtistTree();
            m_worker.Dispose();
            m_worker = null;
        }
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // The progress percentage is a property of e
            processbar.Value = e.ProgressPercentage;
        }
        private void LoadArtistTree()
        {
            List<HierarchicalObjectViewModel> list = new List<HierarchicalObjectViewModel>();
            foreach (Artist artist in m_artistlist)
            {
                HierarchicalObjectViewModel tmp = new HierarchicalObjectViewModel();
                tmp.Name = artist.Name;
                tmp.Image = @"\icons\Artist.png";
                tmp.TextBlockVisible = Visibility.Visible;
                tmp.TextBoxVisible = Visibility.Collapsed;
                list.Add(tmp);
            }
            if (list.Count > 0)
                ArtistTree.ItemsSource = list;
        }
        private void Artisttree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            if (tree.SelectedItem == null)
                return;
            Artist artist = GetArtist(((HierarchicalObjectViewModel)tree.SelectedItem).Name);
            m_currentArtist = artist;
            List<HierarchicalObjectViewModel> list = new List<HierarchicalObjectViewModel>();
            foreach (Album album in artist.Albums)
            {
                HierarchicalObjectViewModel tmp = new HierarchicalObjectViewModel();
                tmp.Name = album.Name;
                tmp.Image = @"\icons\Album.png";
                tmp.TextBlockVisible = Visibility.Visible;
                tmp.TextBoxVisible = Visibility.Collapsed;
                list.Add(tmp);
            }
            AlbumTree.ItemsSource = null;
            SongTree.ItemsSource = null;
            if (list.Count > 0)
                AlbumTree.ItemsSource = list;
        }
        private void Albumtree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            if (tree.SelectedItem == null)
                return;
            Album album = GetAlbum(((HierarchicalObjectViewModel)tree.SelectedItem).Name);
            m_currentAlbum = album;
            List<HierarchicalObjectViewModel> list = new List<HierarchicalObjectViewModel>();
            foreach (Song Song in album.Songs)
            {
                HierarchicalObjectViewModel tmp = new HierarchicalObjectViewModel();
                tmp.Name = Song.Title;
                tmp.Image = @"\icons\song.png";
                tmp.TextBlockVisible = Visibility.Visible;
                tmp.TextBoxVisible = Visibility.Collapsed;
                list.Add(tmp);
            }
            SongTree.ItemsSource = null;
            if (list.Count > 0)
                SongTree.ItemsSource = list;
        }
        private void Songtree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            if (tree.SelectedItem == null)
                return;
            Song song = GetSong(((HierarchicalObjectViewModel)tree.SelectedItem).Name);
            m_currentSong = song;
            
        }
        private Song GetSong(string p)
        {
            var x = from tmpsong in m_songslist where tmpsong.Title == p select tmpsong;

            if (x.Count() == 0)
                return null;
            return x.First();
        }
        public System.Drawing.Bitmap CropImage(System.Drawing.Bitmap source, System.Drawing.Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(section.Width, section.Height);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, System.Drawing.GraphicsUnit.Pixel);

            return bmp;
        }

        
    }
}
