using System;
using System.Collections.Generic;
using System.Text;

namespace SharpJukebox
{
    public interface IPlaylistWriter
    {
        public void Write(Playlist Playlist);
    }
}
