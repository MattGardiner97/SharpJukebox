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
            string path = GetPlaylistPath(Playlist);
            File.WriteAllLines(path, Playlist.Tracks.Select(track => track.Filename));
        }

        public void Delete(Playlist Playlist)
        {
            string path = GetPlaylistPath(Playlist);
            if (File.Exists(path))
                File.Delete(path);
        }

        private string GetPlaylistPath(Playlist Playlist)
        {
            return Path.Join(_playlistPath, Playlist.Name + ".playlist");
        }
    }
}
