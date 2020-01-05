using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SharpJukebox
{
    public class Playlist
    {
        private List<AudioFile> _tracks;

        public string Name { get; set; }
        public ReadOnlyCollection<AudioFile> Tracks { get { return new ReadOnlyCollection<AudioFile>(_tracks); } }

        public Playlist()
        {
            _tracks = new List<AudioFile>();
        }

        public void AddTrack(AudioFile File)
        {
            _tracks.Add(File);
        }
    }
}
