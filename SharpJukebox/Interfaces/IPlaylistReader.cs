using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SharpJukebox
{
    public interface IPlaylistReader
    {
        public void Load();
        public ReadOnlyCollection<Playlist> GetPlaylists();

    }
}
