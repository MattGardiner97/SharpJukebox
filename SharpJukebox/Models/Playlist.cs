using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SharpJukebox
{
    public class Playlist
    {
        private List<AudioFile> _tracks;

        public string Name { get; set; }
        public IEnumerable<AudioFile> Tracks { get { return _tracks; } }

        public Playlist()
        {
            _tracks = new List<AudioFile>();
        }

        public void AddTracks(AudioFile File)
        {
            if (_tracks.Contains(File))
                return;
            _tracks.Add(File);
        }

        public void AddTracks(IEnumerable<AudioFile> Files)
        {
            foreach(AudioFile track in Files)
            {
                if (_tracks.Contains(track))
                    continue;
                _tracks.Add(track);
            }
        }

        public void RemoveTracks(IEnumerable<AudioFile> Tracks)
        {
            foreach (AudioFile file in Tracks)
                _tracks.Remove(file);
        }
    }
}
