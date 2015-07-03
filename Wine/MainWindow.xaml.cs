using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using TagLib;
using Wine.TreeViewModel;

namespace Wine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool m_seekbardragging = false;
        private string[] m_files = null;
        private string[] m_directories = null;
        private BackgroundWorker m_worker = null;
        private List<Artist> m_artistlist = null;
        private List<Song> m_songslist = null;
        private List<Album> m_albumslist = null;
        private MediaPlayer player = null;
        private BitmapImage m_default_albumart;
        private Artist m_currentArtist=null;
        private Album m_currentAlbum=null;
        private Song m_currentSong=null;
        private DispatcherTimer m_timer;
        private SongInfoModel m_currentsonfinfomodel=null;
        public MainWindow()
        {
            InitializeComponent();
            
            m_default_albumart = new BitmapImage();
            m_default_albumart.BeginInit();
            m_default_albumart.UriSource = new Uri("//icons//cd3.png", UriKind.RelativeOrAbsolute);
            m_default_albumart.DecodePixelWidth = 100;
            m_default_albumart.EndInit();

             player = new MediaPlayer();
             player.MediaOpened += player_MediaOpened;
             player.MediaEnded += player_MediaEnded;
             DirectoryAddressTextBox.Text = @"C:\Users\tarunkumar.tomar\Music";
             Refresh_Click(null, null);
            
        }

        void player_MediaEnded(object sender, EventArgs e)
        {
            m_worker.Dispose();
        }

        private void PlaySong()
        {
            // code for play
            if (m_currentSong == null)
                return;

            // Open Media File
            Uri uri = new Uri(m_currentSong.FilePath, UriKind.RelativeOrAbsolute);
            m_timer = new DispatcherTimer();
            m_timer.Interval = TimeSpan.FromSeconds(1);
            m_timer.Tick += m_timer_Tick;
            new LyricsManager(m_currentSong);
            player.Open(uri);
            
            ImageSource source = GetAlbumCover(m_currentSong);
            if (source == null)
                albumartbox.Source = m_default_albumart;
            else
                albumartbox.Source = source;
        }
        private ImageSource GetAlbumCover(Song song)
        {
            BitmapImage image= null;
            TagLib.File tlFile = TagLib.File.Create(song.FilePath);
            if (tlFile.Tag.Pictures.Count() > 0)
            {
                MemoryStream ms = new MemoryStream(tlFile.Tag.Pictures[0].Data.Data);
                ms.Seek(0, SeekOrigin.Begin);

                // ImageSource for System.Windows.Controls.Image
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();
            }
            return image;
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            if (!m_seekbardragging)
            {
                m_seekbar.Value = player.Position.TotalSeconds;
            }
        }

        void player_MediaOpened(object sender, EventArgs e)
        {

            //********Seekbar *********//
            TimeSpan ts = player.NaturalDuration.TimeSpan;
            m_seekbar.Value = 0;
            m_seekbar.Maximum = ts.TotalSeconds;
            m_seekbar.SmallChange = 1;
            //m_seekbar.LargeChange = Math.Min(10, ts.Seconds / 10);
            //*******END OF MEDIA PLAYER SETTING*****//
            Image img = new Image() { Source = new BitmapImage(new Uri("icons\\PauseHS.png", UriKind.RelativeOrAbsolute)) };
            PlayPauseButton.Content = img;
            NowPlayingLabel.Text = m_currentSong.Title;
            TotalTimeLabel.Text = TimeSpan.FromSeconds(player.NaturalDuration.TimeSpan.TotalSeconds).ToString(@"mm\:ss");
            player.Play();
            m_timer.Start();
            LoadSongInfo();
            LoadLyrics();
        }

        private void LoadSongInfo()
        {
            // Titte
            m_songinfodisplaypanel.Children.Add(new TextBlock() { Text="Title:"+ m_currentSong.Title, Margin=new Thickness(5,10,0,0)});

            // Album
            m_songinfodisplaypanel.Children.Add(new TextBlock() { Text = "Album:" + m_currentSong.Album, Margin = new Thickness(5, 10, 0, 0) });

            // Artist
            m_songinfodisplaypanel.Children.Add(new TextBlock() { Text = "Artist:" + m_currentSong.Artist, Margin = new Thickness(5, 10, 0, 0) });

            // File Path
            m_songinfodisplaypanel.Children.Add(new TextBlock() { Text = "File Path:" + m_currentSong.FilePath, Margin = new Thickness(5, 10, 0, 0), TextWrapping=TextWrapping.Wrap });
        }
        private void LoadLyrics()
        {
            m_currentsonfinfomodel = null;
            // check if file is available
            string songpath = m_currentSong.FilePath;
            string fileaddress = songpath.Remove(songpath.Length - 3, 3) + "xml";
            try
            {
                FileStream fw = new FileStream(fileaddress, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                XmlSerializer xmlserializer = new XmlSerializer(typeof(SongInfoModel));
                m_currentsonfinfomodel = (SongInfoModel)xmlserializer.Deserialize(fw);

            }
            catch (Exception e) { }

            // if available then load lyrics
            if (m_currentsonfinfomodel != null)
            {
                m_worker = new BackgroundWorker();
                m_worker.DoWork += UpdateLyricsfromBackgroundWorker;
                m_worker.RunWorkerAsync();
            }
            // show lyrics on Song info tab
        }
        void UpdateLyricsfromBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            //TextBlock tb = new TextBlock() {Margin = new Thickness(0, 0, 0, 10), HorizontalAlignment = HorizontalAlignment.Center };
            //m_songinfodisplaypanel.Children.Add(tb);
            foreach (LyricsLine lyricsline in m_currentsonfinfomodel.Lyrics)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    LyricsLineTextBlock.Text = lyricsline.Line;
                }));
                
                Thread.Sleep(lyricsline.Time * 1000);
                //m_songinfodisplaypanel.Children.Add(new TextBlock() { Text = lyricsline.Line, Margin = new Thickness(0, 0, 0, 10), HorizontalAlignment = HorizontalAlignment.Center });
            }
        }
     
        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            Image img = new Image();
            if(m_timer.IsEnabled)
            {
                player.Pause();
                m_timer.Stop();
                img.Source = new BitmapImage(new Uri("icons\\PlayHS.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                player.Play();
                m_timer.Start();
                img.Source = new BitmapImage(new Uri("icons\\PauseHS.png", UriKind.RelativeOrAbsolute));
            }
            PlayPauseButton.Content = img;
        }

    
        
        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            m_seekbardragging = true;
        }
     
        private void m_seekbar_DragOver(object sender, DragCompletedEventArgs e)
        {
            player.Position = TimeSpan.FromSeconds(m_seekbar.Value);
            m_seekbardragging = false;
        }
    


        private void Stop_Click(object sender, RoutedEventArgs e)
        {

          
        }

        private void SongTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlaySong();
          
        }

        private void m_seekbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!m_seekbardragging)
            {
                player.Position = TimeSpan.FromSeconds(m_seekbar.Value);
            }

        }

        private void volbar_DragStarted(object sender, DragStartedEventArgs e)
        {
         //   player.Volume = volumeslider.Value;
        }

        private void m_volbar_DragOver(object sender, DragCompletedEventArgs e)
        {
           // player.Volume = volumeslider.Value;
        }

        private void m_volbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(player!=null)
            player.Volume = (double) volumeslider.Value;
        }

   
     

       
        
    }
}
