using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Wine
{
      [Serializable()]
    public class SongInfoModel
    {
            public string Title { get; set; }
          
            public string Artists { get; set; }
            public List<LyricsLine> Lyrics { get; set; }
            //public List<LyricsLine> Lyrics { get; set; }

       
    }
     [Serializable()]
    public class LyricsLine{

        public string Line { get; set; }
        public int Time { get; set; }

     
    }
}
