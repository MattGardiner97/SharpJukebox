using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJukebox
{
    public class AudioFile
    {
        public string Filename { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public TimeSpan Duration { get; set; }
        public string DurationString => Duration.ToString("mm\\:ss");
        public bool CurrentlyPlaying { get; set; } = false;

        public override string ToString()
        {
            return Title;
        }
    }
}
