using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SharpJukebox
{
    public class PlaylistManager
    {
        private IPlaylistReader _playlistReader;
        private IPlaylistWriter _playlistWriter;

        public ReadOnlyCollection<Playlist> Playlists { get { return new ReadOnlyCollection<Playlist>(_playlistReader.GetPlaylists()); } }

        public PlaylistManager(IPlaylistReader PlaylistReader,IPlaylistWriter PlaylistWriter)
        {
            _playlistReader = PlaylistReader;
            _playlistWriter = PlaylistWriter;
        }

        public void Load()
        {
             _playlistReader.Load();

        }

        public void SavePlaylist(Playlist Playlist)
        {
            _playlistWriter.Write(Playlist);
        }

    }
}
