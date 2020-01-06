using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace SharpJukebox
{
    public class LocalPlaylistWriter : IPlaylistWriter
    {
        private string _playlistPath;

        public LocalPlaylistWriter(string PlaylistPath)
        {
            _playlistPath = PlaylistPath;
        }

        public void Write(Playlist Playlist)
        {
            string path = Path.Join(_playlistPath,Playlist.Name + ".playlist");
            File.WriteAllLines(path, Playlist.Tracks.Select(track => track.Filename));
        }
    }
}
