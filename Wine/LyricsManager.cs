using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Wine
{
    class LyricsManager
    {
        public LyricsManager(Song song)
        {
            SongInfoModel songinfo = new SongInfoModel() { Lyrics = new List<LyricsLine>() };
            songinfo.Title = "I'll be back";
            songinfo.Artists = "Beatles";
            songinfo.Lyrics.Add(new LyricsLine() { Line = " You know if you break my heart I'll go", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "But I'll be back again", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "Cos I told you once before goodbye", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "But I came back again", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "I love you so", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "I'm the one who wants you", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "Yes, I'm the one", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "Who wants you, oh ho, oh ho, oh", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "You could find better things to do", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "Than to break my heart again", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "This time I will try to show that I'm", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "Not trying to pretend", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "I thought that you would realize", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "That if I ran away from you", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "That you would want me too", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "But I got a big surprise", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "Oh ho, oh ho, oh", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "You could find better things to do", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "Than to break my heart again", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "This time I will try to show that I'm", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "Not trying to pretend", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "I wanna go but I hate to leave you,", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "You know I hate to leave you , oh ho, oh ho, oh", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "You, if you break my heart I'll go", Time = 2 });
            songinfo.Lyrics.Add(new LyricsLine() { Line = "But I'll be back again", Time = 2 });
            
            XmlSerializer xmlserializer = new XmlSerializer(typeof(SongInfoModel));
            string songpath = song.FilePath;
            string fileaddress = songpath.Remove(songpath.Length - 3, 3) + "xml";
            StreamWriter sw = new StreamWriter(fileaddress);
                xmlserializer.Serialize(sw,songinfo);
                sw.Close();
        }

        public object Lyrics { get; set; }
    }
}
