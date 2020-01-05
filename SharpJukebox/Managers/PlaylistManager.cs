using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SharpJukebox
{
    public class PlaylistManager
    {
        private IPlaylistReader _playlistReader;

        public ReadOnlyCollection<Playlist> Playlists { get { return new ReadOnlyCollection<Playlist>(_playlistReader.GetPlaylists()); } }

        public PlaylistManager(IPlaylistReader PlaylistReader)
        {
            _playlistReader = PlaylistReader;
        }

        public void Load()
        {
             _playlistReader.Load();

        }

    }
}
