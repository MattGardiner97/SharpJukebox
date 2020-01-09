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

        public IEnumerable<Playlist> Playlists { get { return _playlistReader.GetPlaylists(); } }

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

        public Playlist CreatePlaylist(string PlaylistName)
        {
            Playlist newPlaylist = new Playlist();
            newPlaylist.Name = PlaylistName;
            _playlistWriter.Write(newPlaylist);
            _playlistReader.Load();
            return newPlaylist;
        }

        public void DeletePlaylist(Playlist Playlist)
        {
            _playlistWriter.Delete(Playlist);
        }
    }
}
