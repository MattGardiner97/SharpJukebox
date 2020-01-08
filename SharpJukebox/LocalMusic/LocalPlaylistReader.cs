using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;

namespace SharpJukebox
{
    public class LocalPlaylistReader : IPlaylistReader
    {
        private List<Playlist> _playlists;
        private LocalLibraryManager _localLibraryManager;
        private string _searchLocation;

        public LocalPlaylistReader(LocalLibraryManager LocalLibraryManager, string SearchLocation)
        {
            _localLibraryManager = LocalLibraryManager;
            _playlists = new List<Playlist>();
            _searchLocation = SearchLocation;
        }

        public ReadOnlyCollection<Playlist> GetPlaylists()
        {
            return new ReadOnlyCollection<Playlist>(_playlists);
        }

        public void Load()
        {
            _playlists.Clear();

            if (Directory.Exists(_searchLocation) == false)
                return;

            string[] files = Directory.GetFiles(_searchLocation,"*.playlist");
            foreach(string filename in files)
            {
                Playlist newPlaylist = ParsePlaylistFile(filename);
                _playlists.Add(newPlaylist);
            }
        }

        private Playlist ParsePlaylistFile(string Filename)
        {
            Playlist newPlaylist = new Playlist();
            FileInfo fileInfo = new FileInfo(Filename);

            string playlistName = fileInfo.Name.Replace(fileInfo.Extension, "");
            newPlaylist.Name = playlistName;

            string[] trackFilenames = File.ReadAllLines(Filename);
            foreach (string trackFilename in trackFilenames)
            {
                AudioFile track = _localLibraryManager.FindByFilename(trackFilename);
                newPlaylist.AddTracks(track);
            }

            return newPlaylist;
        }
    }
}
