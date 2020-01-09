using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJukebox
{
    public class LocalLibraryManager
    {
        private ITrackLocater _fileLocater;
        private MetadataExtractor _metadataExtractor;

        public IEnumerable<AudioFile> Tracks { get; private set; }

        public LocalLibraryManager(ITrackLocater FileLocater, MetadataExtractor MetadataExtractor)
        {
            _fileLocater = FileLocater;
            _metadataExtractor = MetadataExtractor;
        }

        public void UpdateFiles()
        {
            _fileLocater.LocateFiles();
            _metadataExtractor.Extract(_fileLocater.GetFiles());
            Tracks = _metadataExtractor.Files;
        }

        public IEnumerable<AudioFile> Search(string Query)
        {
            var result = Tracks.Where(file =>
            file.Title.Contains(Query, StringComparison.OrdinalIgnoreCase) ||
            file.Artist.Contains(Query, StringComparison.OrdinalIgnoreCase) ||
            file.Album.Contains(Query, StringComparison.OrdinalIgnoreCase)
            );
            return result;
        }

        public AudioFile FindByFilename(string Filename)
        {
            foreach(AudioFile track in Tracks)
                if (track.Filename == Filename)
                    return track;

            return null;
        }

        public IEnumerable<AudioFile> FindByArtist(string ArtistName)
        {
            var result = Tracks.Where(file => file.Artist == ArtistName);
            return result;
        }

        public IEnumerable<AudioFile> FindByAlbum(string Artist, string Album)
        {
            var result = Tracks.Where(file => file.Artist == Artist && file.Album == Album);
            return result;
        }

        public IEnumerable<string> GetArtists()
        {
            return Tracks.Select(track => track.Artist).Distinct();
        }
    }
}
